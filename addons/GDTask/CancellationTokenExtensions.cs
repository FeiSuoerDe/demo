using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace demo.addons.GDTask
{
    public static class CancellationTokenExtensions
    {
        private static readonly Action<object>? CancellationTokenCallback = Callback;
        private static readonly Action<object>? disposeCallback = DisposeCallback;

        public static CancellationToken ToCancellationToken(this GDTask task)
        {
            var cts = new CancellationTokenSource();
            ToCancellationTokenCore(task, cts).Forget();
            return cts.Token;
        }

        public static CancellationToken ToCancellationToken(this GDTask task, CancellationToken linkToken)
        {
            if (linkToken.IsCancellationRequested)
            {
                return linkToken;
            }

            if (!linkToken.CanBeCanceled)
            {
                return ToCancellationToken(task);
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(linkToken);
            ToCancellationTokenCore(task, cts).Forget();

            return cts.Token;
        }

        public static CancellationToken ToCancellationToken<T>(this GdTask<T> task)
        {
            return ToCancellationToken(task.AsGdTask());
        }

        public static CancellationToken ToCancellationToken<T>(this GdTask<T> task, CancellationToken linkToken)
        {
            return ToCancellationToken(task.AsGdTask(), linkToken);
        }

        private static async GDTaskVoid ToCancellationTokenCore(GDTask task, CancellationTokenSource cts)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                GdTaskScheduler.PublishUnobservedTaskException(ex);
            }
            cts.Cancel();
            cts.Dispose();
        }

        public static (GDTask, CancellationTokenRegistration) ToGdTask(this CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return (GDTask.FromCanceled(cancellationToken), default(CancellationTokenRegistration));
            }

            var promise = new GdTaskCompletionSource();
            return (promise.Task, cancellationToken.RegisterWithoutCaptureExecutionContext(CancellationTokenCallback, promise));
        }

        private static void Callback(object state)
        {
            var promise = (GdTaskCompletionSource)state;
            promise.TrySetResult();
        }

        public static CancellationTokenAwaitable WaitUntilCanceled(this CancellationToken cancellationToken)
        {
            return new CancellationTokenAwaitable(cancellationToken);
        }

        public static CancellationTokenRegistration RegisterWithoutCaptureExecutionContext(this CancellationToken cancellationToken, Action callback)
        {
            var restoreFlow = false;
            if (!ExecutionContext.IsFlowSuppressed())
            {
                ExecutionContext.SuppressFlow();
                restoreFlow = true;
            }

            try
            {
                return cancellationToken.Register(callback, false);
            }
            finally
            {
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                }
            }
        }

        public static CancellationTokenRegistration RegisterWithoutCaptureExecutionContext(this CancellationToken cancellationToken, Action<object>? callback, object state)
        {
            var restoreFlow = false;
            if (!ExecutionContext.IsFlowSuppressed())
            {
                ExecutionContext.SuppressFlow();
                restoreFlow = true;
            }

            try
            {
                return cancellationToken.Register(callback, state, false);
            }
            finally
            {
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                }
            }
        }

        public static CancellationTokenRegistration AddTo(this IDisposable disposable, CancellationToken cancellationToken)
        {
            return cancellationToken.RegisterWithoutCaptureExecutionContext(disposeCallback, disposable);
        }

        private static void DisposeCallback(object state)
        {
            var d = (IDisposable)state;
            d.Dispose();
        }
    }

    public struct CancellationTokenAwaitable
    {
        private CancellationToken _cancellationToken;

        public CancellationTokenAwaitable(CancellationToken cancellationToken)
        {
            this._cancellationToken = cancellationToken;
        }

        public Awaiter GetAwaiter()
        {
            return new Awaiter(_cancellationToken);
        }

        public struct Awaiter : ICriticalNotifyCompletion
        {
            private CancellationToken _cancellationToken;

            public Awaiter(CancellationToken cancellationToken)
            {
                this._cancellationToken = cancellationToken;
            }

            public bool IsCompleted => !_cancellationToken.CanBeCanceled || _cancellationToken.IsCancellationRequested;

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                _cancellationToken.RegisterWithoutCaptureExecutionContext(continuation);
            }
        }
    }
}

