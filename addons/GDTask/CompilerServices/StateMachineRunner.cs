#pragma warning disable CS1591

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using demo.addons.GDTask.Internal;

namespace demo.addons.GDTask.CompilerServices
{
    internal interface IStateMachineRunner
    {
        Action MoveNext { get; }
        void Return();

        Action ReturnAction { get; }
    }

    internal interface IStateMachineRunnerPromise : IGdTaskSource
    {
        Action MoveNext { get; }
        GDTask Task { get; }
        void SetResult();
        void SetException(Exception? exception);
    }

    internal interface IStateMachineRunnerPromise<T> : IGdTaskSource<T>
    {
        Action MoveNext { get; }
        GdTask<T?> Task { get; }
        void SetResult(T? result);
        void SetException(Exception? exception);
    }

    internal static class StateMachineUtility
    {
        // Get AsyncStateMachine internal state to check IL2CPP bug
        public static int GetState(IAsyncStateMachine stateMachine)
        {
            var info = stateMachine.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .First(x => x.Name.EndsWith("__state"));
            return (int)info.GetValue(stateMachine);
        }
    }

    internal sealed class AsyncGdTaskVoid<TStateMachine> : IStateMachineRunner, ITaskPoolNode<AsyncGdTaskVoid<TStateMachine>>, IGdTaskSource
        where TStateMachine : IAsyncStateMachine
    {
        private static TaskPool<AsyncGdTaskVoid<TStateMachine>> _pool;

        public Action ReturnAction { get; }

        private TStateMachine _stateMachine;

        public Action MoveNext { get; }

        public AsyncGdTaskVoid()
        {
            MoveNext = Run;
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunner runnerFieldRef)
        {
            if (!_pool.TryPop(out var result))
            {
                result = new AsyncGdTaskVoid<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerFieldRef = result; // set runner before copied.
            result._stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        static AsyncGdTaskVoid()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncGdTaskVoid<TStateMachine>), () => _pool.Size);
        }

        private AsyncGdTaskVoid<TStateMachine> _nextNode;
        public ref AsyncGdTaskVoid<TStateMachine> NextNode => ref _nextNode;

        public void Return()
        {
            TaskTracker.RemoveTracking(this);
            _stateMachine = default;
            _pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Run()
        {
            _stateMachine.MoveNext();
        }

        // dummy interface implementation for TaskTracker.

        GdTaskStatus IGdTaskSource.GetStatus(short token)
        {
            return GdTaskStatus.Pending;
        }

        GdTaskStatus IGdTaskSource.UnsafeGetStatus()
        {
            return GdTaskStatus.Pending;
        }

        void IGdTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
        }

        void IGdTaskSource.GetResult(short token)
        {
        }
    }

    internal sealed class AsyncGdTask<TStateMachine> : IStateMachineRunnerPromise, IGdTaskSource, ITaskPoolNode<AsyncGdTask<TStateMachine>>
        where TStateMachine : IAsyncStateMachine
    {
        private static TaskPool<AsyncGdTask<TStateMachine>> _pool;
        public Action MoveNext { get; }

        private TStateMachine _stateMachine;
        private GdTaskCompletionSourceCore<AsyncUnit> _core;

        private AsyncGdTask()
        {
            MoveNext = Run;
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise runnerPromiseFieldRef)
        {
            if (!_pool.TryPop(out var result))
            {
                result = new AsyncGdTask<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result._stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        private AsyncGdTask<TStateMachine> _nextNode;
        public ref AsyncGdTask<TStateMachine> NextNode => ref _nextNode;

        static AsyncGdTask()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncGdTask<TStateMachine>), () => _pool.Size);
        }

        private void Return()
        {
            TaskTracker.RemoveTracking(this);
            _core.Reset();
            _stateMachine = default;
            _pool.TryPush(this);
        }

        private bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            _core.Reset();
            _stateMachine = default;
            return _pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Run()
        {
            _stateMachine.MoveNext();
        }

        public GDTask Task
        {
            [DebuggerHidden]
            get
            {
                return new GDTask(this, _core.Version);
            }
        }

        [DebuggerHidden]
        public void SetResult()
        {
            _core.TrySetResult(AsyncUnit.Default);
        }

        [DebuggerHidden]
        public void SetException(Exception? exception)
        {
            _core.TrySetException(exception);
        }

        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                _core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public GdTaskStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        [DebuggerHidden]
        public GdTaskStatus UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            _core.OnCompleted(continuation, state, token);
        }
    }

    internal sealed class AsyncGdTask<TStateMachine, T> : IStateMachineRunnerPromise<T>, IGdTaskSource<T>, ITaskPoolNode<AsyncGdTask<TStateMachine, T>>
        where TStateMachine : IAsyncStateMachine
    {
        private static TaskPool<AsyncGdTask<TStateMachine, T>> _pool;

        public Action MoveNext { get; }

        private TStateMachine _stateMachine;
        private GdTaskCompletionSourceCore<T?> _core;

        private AsyncGdTask()
        {
            MoveNext = Run;
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise<T> runnerPromiseFieldRef)
        {
            if (!_pool.TryPop(out var result))
            {
                result = new AsyncGdTask<TStateMachine, T>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result._stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        private AsyncGdTask<TStateMachine, T> _nextNode;
        public ref AsyncGdTask<TStateMachine, T> NextNode => ref _nextNode;

        static AsyncGdTask()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncGdTask<TStateMachine, T>), () => _pool.Size);
        }

        private void Return()
        {
            TaskTracker.RemoveTracking(this);
            _core.Reset();
            _stateMachine = default;
            _pool.TryPush(this);
        }

        private bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            _core.Reset();
            _stateMachine = default;
            return _pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Run()
        {
            _stateMachine.MoveNext();
        }

        public GdTask<T?> Task
        {
            [DebuggerHidden]
            get
            {
                return new GdTask<T?>(this, _core.Version);
            }
        }

        [DebuggerHidden]
        public void SetResult(T? result)
        {
            _core.TrySetResult(result);
        }

        [DebuggerHidden]
        public void SetException(Exception? exception)
        {
            _core.TrySetException(exception);
        }

        [DebuggerHidden]
        public T? GetResult(short token)
        {
            try
            {
                return _core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        void IGdTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        [DebuggerHidden]
        public GdTaskStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        [DebuggerHidden]
        public GdTaskStatus UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            _core.OnCompleted(continuation, state, token);
        }
    }
}

