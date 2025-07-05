using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace demo.addons.GDTask
{
    public partial struct GDTask
    {
        private static readonly GDTask CanceledGDTask = new Func<GDTask>(() =>
        {
            return new GDTask(new CanceledResultSource(CancellationToken.None), 0);
        })();

        private static class CanceledGdTaskCache<T>
        {
            public static readonly GdTask<T?> Task;

            static CanceledGdTaskCache()
            {
                Task = new GdTask<T?>(new CanceledResultSource<T>(CancellationToken.None), 0);
            }
        }

        public static readonly GDTask CompletedTask = new GDTask();

        public static GDTask FromException(Exception? ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled(oce.CancellationToken);
            }

            return new GDTask(new ExceptionResultSource(ex), 0);
        }

        public static GdTask<T?> FromException<T>(Exception? ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled<T>(oce.CancellationToken);
            }

            return new GdTask<T?>(new ExceptionResultSource<T>(ex), 0);
        }

        public static GdTask<T?> FromResult<T>(T value)
        {
            return new GdTask<T?>(value);
        }

        public static GDTask FromCanceled(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledGDTask;
            }
            else
            {
                return new GDTask(new CanceledResultSource(cancellationToken), 0);
            }
        }

        public static GdTask<T?> FromCanceled<T>(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledGdTaskCache<T>.Task;
            }
            else
            {
                return new GdTask<T?>(new CanceledResultSource<T>(cancellationToken), 0);
            }
        }

        public static GDTask Create(Func<GDTask> factory)
        {
            return factory();
        }

        public static GdTask<T> Create<T>(Func<GdTask<T>> factory)
        {
            return factory();
        }

        public static AsyncLazy Lazy(Func<GDTask> factory)
        {
            return new AsyncLazy(factory);
        }

        public static AsyncLazy<T> Lazy<T>(Func<GdTask<T>> factory)
        {
            return new AsyncLazy<T>(factory);
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<GDTaskVoid> asyncAction)
        {
            asyncAction().Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<CancellationToken, GDTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void<T>(Func<T, GDTaskVoid> asyncAction, T state)
        {
            asyncAction(state).Forget();
        }

        /// <summary>
        /// helper of create add GDTaskVoid to delegate.
        /// For example: FooAction = GDTask.Action(async () => { /* */ })
        /// </summary>
        public static Action Action(Func<GDTaskVoid> asyncAction)
        {
            return () => asyncAction().Forget();
        }

        /// <summary>
        /// helper of create add GDTaskVoid to delegate.
        /// </summary>
        public static Action Action(Func<CancellationToken, GDTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return () => asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static GDTask Defer(Func<GDTask> factory)
        {
            return new GDTask(new DeferPromise(factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static GdTask<T?> Defer<T>(Func<GdTask<T>> factory)
        {
            return new GdTask<T?>(new DeferPromise<T>(factory), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static GDTask Never(CancellationToken cancellationToken)
        {
            return new GdTask<AsyncUnit>(new NeverPromise<AsyncUnit>(cancellationToken), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static GdTask<T?> Never<T>(CancellationToken cancellationToken)
        {
            return new GdTask<T?>(new NeverPromise<T>(cancellationToken), 0);
        }

        private sealed class ExceptionResultSource : IGdTaskSource
        {
            private readonly ExceptionDispatchInfo _exception;
            private bool _calledGet;

            public ExceptionResultSource(Exception? exception)
            {
                this._exception = ExceptionDispatchInfo.Capture(exception);
            }

            public void GetResult(short token)
            {
                if (!_calledGet)
                {
                    _calledGet = true;
                    GC.SuppressFinalize(this);
                }
                _exception.Throw();
            }

            public GdTaskStatus GetStatus(short token)
            {
                return GdTaskStatus.Faulted;
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return GdTaskStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!_calledGet)
                {
                    GdTaskScheduler.PublishUnobservedTaskException(_exception.SourceException);
                }
            }
        }

        private sealed class ExceptionResultSource<T> : IGdTaskSource<T>
        {
            private readonly ExceptionDispatchInfo _exception;
            private bool _calledGet;

            public ExceptionResultSource(Exception? exception)
            {
                this._exception = ExceptionDispatchInfo.Capture(exception);
            }

            public T? GetResult(short token)
            {
                if (!_calledGet)
                {
                    _calledGet = true;
                    GC.SuppressFinalize(this);
                }
                _exception.Throw();
                return default;
            }

            void IGdTaskSource.GetResult(short token)
            {
                if (!_calledGet)
                {
                    _calledGet = true;
                    GC.SuppressFinalize(this);
                }
                _exception.Throw();
            }

            public GdTaskStatus GetStatus(short token)
            {
                return GdTaskStatus.Faulted;
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return GdTaskStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!_calledGet)
                {
                    GdTaskScheduler.PublishUnobservedTaskException(_exception.SourceException);
                }
            }
        }

        private sealed class CanceledResultSource : IGdTaskSource
        {
            private readonly CancellationToken _cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this._cancellationToken = cancellationToken;
            }

            public void GetResult(short token)
            {
                throw new OperationCanceledException(_cancellationToken);
            }

            public GdTaskStatus GetStatus(short token)
            {
                return GdTaskStatus.Canceled;
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return GdTaskStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        private sealed class CanceledResultSource<T> : IGdTaskSource<T>
        {
            private readonly CancellationToken _cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this._cancellationToken = cancellationToken;
            }

            public T? GetResult(short token)
            {
                throw new OperationCanceledException(_cancellationToken);
            }

            void IGdTaskSource.GetResult(short token)
            {
                throw new OperationCanceledException(_cancellationToken);
            }

            public GdTaskStatus GetStatus(short token)
            {
                return GdTaskStatus.Canceled;
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return GdTaskStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        private sealed class DeferPromise : IGdTaskSource
        {
            private Func<GDTask> _factory;
            private GDTask _task;
            private GDTask.Awaiter _awaiter;

            public DeferPromise(Func<GDTask> factory)
            {
                this._factory = factory;
            }

            public void GetResult(short token)
            {
                _awaiter.GetResult();
            }

            public GdTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref _factory, null);
                if (f != null)
                {
                    _task = f();
                    _awaiter = _task.GetAwaiter();
                }

                return _task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _awaiter.SourceOnCompleted(continuation, state);
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return _task.Status;
            }
        }

        private sealed class DeferPromise<T> : IGdTaskSource<T>
        {
            private Func<GdTask<T>> _factory;
            private GdTask<T?> _task;
            private GdTask<T?>.Awaiter _awaiter;

            public DeferPromise(Func<GdTask<T>> factory)
            {
                this._factory = factory;
            }

            public T? GetResult(short token)
            {
                return _awaiter.GetResult();
            }

            void IGdTaskSource.GetResult(short token)
            {
                _awaiter.GetResult();
            }

            public GdTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref _factory, null);
                if (f != null)
                {
                    _task = f();
                    _awaiter = _task.GetAwaiter();
                }

                return _task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _awaiter.SourceOnCompleted(continuation, state);
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return _task.Status;
            }
        }

        private sealed class NeverPromise<T> : IGdTaskSource<T>
        {
            private static readonly Action<object>? cancellationCallback = CancellationCallback;

            private CancellationToken _cancellationToken;
            private GdTaskCompletionSourceCore<T?> _core;

            public NeverPromise(CancellationToken cancellationToken)
            {
                this._cancellationToken = cancellationToken;
                if (this._cancellationToken.CanBeCanceled)
                {
                    this._cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            private static void CancellationCallback(object state)
            {
                var self = (NeverPromise<T?>)state;
                self._core.TrySetCanceled(self._cancellationToken);
            }

            public T? GetResult(short token)
            {
                return _core.GetResult(token);
            }

            public GdTaskStatus GetStatus(short token)
            {
                return _core.GetStatus(token);
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return _core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _core.OnCompleted(continuation, state, token);
            }

            void IGdTaskSource.GetResult(short token)
            {
                _core.GetResult(token);
            }
        }
    }

    internal static class CompletedTasks
    {
        public static readonly GdTask<AsyncUnit> AsyncUnit = GDTask.FromResult(addons.GDTask.AsyncUnit.Default);
        public static readonly GdTask<bool> True = GDTask.FromResult(true);
        public static readonly GdTask<bool> False = GDTask.FromResult(false);
        public static readonly GdTask<int> Zero = GDTask.FromResult(0);
        public static readonly GdTask<int> MinusOne = GDTask.FromResult(-1);
        public static readonly GdTask<int> One = GDTask.FromResult(1);
    }
}
