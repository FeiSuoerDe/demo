using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace demo.addons.GDTask.CompilerServices
{
    [StructLayout(LayoutKind.Auto)]
    public struct AsyncGdTaskMethodBuilder
    {
        private IStateMachineRunnerPromise runnerPromise;
        private Exception? ex;

        // 1. Static Create method.
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncGdTaskMethodBuilder Create()
        {
            return default;
        }

        // 2. TaskLike Task property.
        public GDTask Task
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (runnerPromise != null)
                {
                    return runnerPromise.Task;
                }
                else if (ex != null)
                {
                    return GDTask.FromException(ex);
                }
                else
                {
                    return GDTask.CompletedTask;
                }
            }
        }

        // 3. SetException
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception? exception)
        {
            if (runnerPromise == null)
            {
                ex = exception;
            }
            else
            {
                runnerPromise.SetException(exception);
            }
        }

        // 4. SetResult
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult()
        {
            if (runnerPromise != null)
            {
                runnerPromise.SetResult();
            }
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (runnerPromise == null)
            {
                AsyncGdTask<TStateMachine>.SetStateMachine(ref stateMachine, ref runnerPromise);
            }

            awaiter.OnCompleted(runnerPromise.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (runnerPromise == null)
            {
                AsyncGdTask<TStateMachine>.SetStateMachine(ref stateMachine, ref runnerPromise);
            }

            awaiter.UnsafeOnCompleted(runnerPromise.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // don't use boxed stateMachine.
        }

#if DEBUG
        // Important for IDE debugger.
        private object debuggingId;
        private object ObjectIdForDebugger
        {
            get
            {
                if (debuggingId == null)
                {
                    debuggingId = new object();
                }
                return debuggingId;
            }
        }
#endif
    }

    [StructLayout(LayoutKind.Auto)]
    public struct AsyncGdTaskMethodBuilder<T>
    {
        private IStateMachineRunnerPromise<T?> runnerPromise;
        private Exception? ex;
        private T? result;

        // 1. Static Create method.
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncGdTaskMethodBuilder<T> Create()
        {
            return default;
        }

        // 2. TaskLike Task property.
        public GdTask<T?> Task
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (runnerPromise != null)
                {
                    return runnerPromise.Task;
                }
                else if (ex != null)
                {
                    return GDTask.FromException<T>(ex);
                }
                else
                {
                    return GDTask.FromResult(result);
                }
            }
        }

        // 3. SetException
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception? exception)
        {
            if (runnerPromise == null)
            {
                ex = exception;
            }
            else
            {
                runnerPromise.SetException(exception);
            }
        }

        // 4. SetResult
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(T? result)
        {
            if (runnerPromise == null)
            {
                this.result = result;
            }
            else
            {
                runnerPromise.SetResult(result);
            }
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (runnerPromise == null)
            {
                AsyncGdTask<TStateMachine, T>.SetStateMachine(ref stateMachine, ref runnerPromise);
            }

            awaiter.OnCompleted(runnerPromise.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (runnerPromise == null)
            {
                AsyncGdTask<TStateMachine, T>.SetStateMachine(ref stateMachine, ref runnerPromise);
            }

            awaiter.UnsafeOnCompleted(runnerPromise.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // don't use boxed stateMachine.
        }

#if DEBUG
        // Important for IDE debugger.
        private object debuggingId;
        private object ObjectIdForDebugger
        {
            get
            {
                if (debuggingId == null)
                {
                    debuggingId = new object();
                }
                return debuggingId;
            }
        }
#endif

    }
}