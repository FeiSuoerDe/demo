using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using demo.addons.GDTask.CompilerServices;

namespace demo.addons.GDTask
{
    internal static class AwaiterActions
    {
        internal static readonly Action<object> InvokeContinuationDelegate = Continuation;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Continuation(object state)
        {
            ((Action)state).Invoke();
        }
    }

    /// <summary>
    /// Lightweight Godot specific task-like object with a void return value.
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncGdTaskMethodBuilder))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct GDTask
    {
        private readonly IGdTaskSource source;
        private readonly short token;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GDTask(IGdTaskSource source, short token)
        {
            this.source = source;
            this.token = token;
        }

        public GdTaskStatus Status
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (source == null) return GdTaskStatus.Succeeded;
                return source.GetStatus(token);
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }

        /// <summary>
        /// returns (bool IsCanceled) instead of throws OperationCanceledException.
        /// </summary>
        public GdTask<bool> SuppressCancellationThrow()
        {
            var status = Status;
            if (status == GdTaskStatus.Succeeded) return CompletedTasks.False;
            if (status == GdTaskStatus.Canceled) return CompletedTasks.True;
            return new GdTask<bool>(new IsCanceledSource(source), token);
        }
        public override string ToString()
        {
            if (source == null) return "()";
            return "(" + source.UnsafeGetStatus() + ")";
        }

        /// <summary>
        /// Memoizing inner IValueTaskSource. The result GDTask can await multiple.
        /// </summary>
        public GDTask Preserve()
        {
            if (source == null)
            {
                return this;
            }
            else
            {
                return new GDTask(new MemoizeSource(source), token);
            }
        }

        public GdTask<AsyncUnit> AsAsyncUnitGdTask()
        {
            if (this.source == null) return CompletedTasks.AsyncUnit;

            var status = this.source.GetStatus(this.token);
            if (status.IsCompletedSuccessfully())
            {
                this.source.GetResult(this.token);
                return CompletedTasks.AsyncUnit;
            }
            else if (this.source is IGdTaskSource<AsyncUnit> asyncUnitSource)
            {
                return new GdTask<AsyncUnit>(asyncUnitSource, this.token);
            }

            return new GdTask<AsyncUnit>(new AsyncUnitSource(this.source), this.token);
        }

        private sealed class AsyncUnitSource : IGdTaskSource<AsyncUnit>
        {
            private readonly IGdTaskSource _source;

            public AsyncUnitSource(IGdTaskSource source)
            {
                this._source = source;
            }

            public AsyncUnit GetResult(short token)
            {
                _source.GetResult(token);
                return AsyncUnit.Default;
            }

            public GdTaskStatus GetStatus(short token)
            {
                return _source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _source.OnCompleted(continuation, state, token);
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return _source.UnsafeGetStatus();
            }

            void IGdTaskSource.GetResult(short token)
            {
                GetResult(token);
            }
        }

        private sealed class IsCanceledSource : IGdTaskSource<bool>
        {
            private readonly IGdTaskSource _source;

            public IsCanceledSource(IGdTaskSource source)
            {
                this._source = source;
            }

            public bool GetResult(short token)
            {
                if (_source.GetStatus(token) == GdTaskStatus.Canceled)
                {
                    return true;
                }

                _source.GetResult(token);
                return false;
            }

            void IGdTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public GdTaskStatus GetStatus(short token)
            {
                return _source.GetStatus(token);
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                return _source.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _source.OnCompleted(continuation, state, token);
            }
        }

        private sealed class MemoizeSource : IGdTaskSource
        {
            private IGdTaskSource _source;
            private ExceptionDispatchInfo _exception;
            private GdTaskStatus _status;

            public MemoizeSource(IGdTaskSource source)
            {
                this._source = source;
            }

            public void GetResult(short token)
            {
                if (_source == null)
                {
                    if (_exception != null)
                    {
                        _exception.Throw();
                    }
                }
                else
                {
                    try
                    {
                        _source.GetResult(token);
                        _status = GdTaskStatus.Succeeded;
                    }
                    catch (Exception ex)
                    {
                        _exception = ExceptionDispatchInfo.Capture(ex);
                        if (ex is OperationCanceledException)
                        {
                            _status = GdTaskStatus.Canceled;
                        }
                        else
                        {
                            _status = GdTaskStatus.Faulted;
                        }
                        throw;
                    }
                    finally
                    {
                        _source = null;
                    }
                }
            }

            public GdTaskStatus GetStatus(short token)
            {
                if (_source == null)
                {
                    return _status;
                }

                return _source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                if (_source == null)
                {
                    continuation(state);
                }
                else
                {
                    _source.OnCompleted(continuation, state, token);
                }
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                if (_source == null)
                {
                    return _status;
                }

                return _source.UnsafeGetStatus();
            }
        }

        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            private readonly GDTask _task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in GDTask task)
            {
                this._task = task;
            }

            public bool IsCompleted
            {
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return _task.Status.IsCompleted();
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult()
            {
                if (_task.source == null) return;
                _task.source.GetResult(_task.token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                if (_task.source == null)
                {
                    continuation();
                }
                else
                {
                    _task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, _task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (_task.source == null)
                {
                    continuation();
                }
                else
                {
                    _task.source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, _task.token);
                }
            }

            /// <summary>
            /// If register manually continuation, you can use it instead of for compiler OnCompleted methods.
            /// </summary>
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SourceOnCompleted(Action<object> continuation, object state)
            {
                if (_task.source == null)
                {
                    continuation(state);
                }
                else
                {
                    _task.source.OnCompleted(continuation, state, _task.token);
                }
            }
        }
    }

    /// <summary>
    /// Lightweight Godot specified task-like object with a return value.
    /// </summary>
    /// <typeparam name="T">Return value of the task</typeparam>
    [AsyncMethodBuilder(typeof(AsyncGdTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct GdTask<T>
    {
        private readonly IGdTaskSource<T?> source;
        private readonly T? result;
        private readonly short token;

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GdTask(T? result)
        {
            this.source = default;
            this.token = default;
            this.result = result;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GdTask(IGdTaskSource<T?> source, short token)
        {
            this.source = source;
            this.token = token;
            this.result = default;
        }

        public GdTaskStatus Status
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (source == null) ? GdTaskStatus.Succeeded : source.GetStatus(token);
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }

        /// <summary>
        /// Memoizing inner IValueTaskSource. The result GDTask can await multiple.
        /// </summary>
        public GdTask<T?> Preserve()
        {
            if (source == null)
            {
                return this;
            }
            else
            {
                return new GdTask<T?>(new MemoizeSource(source), token);
            }
        }

        public GDTask AsGdTask()
        {
            if (this.source == null) return GDTask.CompletedTask;

            var status = this.source.GetStatus(this.token);
            if (status.IsCompletedSuccessfully())
            {
                this.source.GetResult(this.token);
                return GDTask.CompletedTask;
            }

            // Converting GDTask<T> -> GDTask is zero overhead.
            return new GDTask(this.source, this.token);
        }

        public static implicit operator GDTask(GdTask<T> self)
        {
            return self.AsGdTask();
        }

        /// <summary>
        /// returns (bool IsCanceled, T Result) instead of throws OperationCanceledException.
        /// </summary>
        public GdTask<(bool IsCanceled, T Result)> SuppressCancellationThrow()
        {
            if (source == null)
            {
                return new GdTask<(bool IsCanceled, T Result)>((false, result));
            }

            return new GdTask<(bool, T)>(new IsCanceledSource(source), token);
        }

        public override string ToString()
        {
            return (this.source == null) ? result?.ToString()
                 : "(" + this.source.UnsafeGetStatus() + ")";
        }

        private sealed class IsCanceledSource : IGdTaskSource<(bool, T)>
        {
            private readonly IGdTaskSource<T?> _source;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IsCanceledSource(IGdTaskSource<T?> source)
            {
                this._source = source;
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (bool, T) GetResult(short token)
            {
                if (_source.GetStatus(token) == GdTaskStatus.Canceled)
                {
                    return (true, default);
                }

                var result = _source.GetResult(token);
                return (false, result);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IGdTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public GdTaskStatus GetStatus(short token)
            {
                return _source.GetStatus(token);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public GdTaskStatus UnsafeGetStatus()
            {
                return _source.UnsafeGetStatus();
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                _source.OnCompleted(continuation, state, token);
            }
        }

        private sealed class MemoizeSource : IGdTaskSource<T>
        {
            private IGdTaskSource<T?> _source;
            private T? _result;
            private ExceptionDispatchInfo _exception;
            private GdTaskStatus _status;

            public MemoizeSource(IGdTaskSource<T?> source)
            {
                this._source = source;
            }

            public T? GetResult(short token)
            {
                if (_source == null)
                {
                    if (_exception != null)
                    {
                        _exception.Throw();
                    }
                    return _result;
                }
                else
                {
                    try
                    {
                        _result = _source.GetResult(token);
                        _status = GdTaskStatus.Succeeded;
                        return _result;
                    }
                    catch (Exception ex)
                    {
                        _exception = ExceptionDispatchInfo.Capture(ex);
                        if (ex is OperationCanceledException)
                        {
                            _status = GdTaskStatus.Canceled;
                        }
                        else
                        {
                            _status = GdTaskStatus.Faulted;
                        }
                        throw;
                    }
                    finally
                    {
                        _source = null;
                    }
                }
            }

            void IGdTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public GdTaskStatus GetStatus(short token)
            {
                if (_source == null)
                {
                    return _status;
                }

                return _source.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                if (_source == null)
                {
                    continuation(state);
                }
                else
                {
                    _source.OnCompleted(continuation, state, token);
                }
            }

            public GdTaskStatus UnsafeGetStatus()
            {
                if (_source == null)
                {
                    return _status;
                }

                return _source.UnsafeGetStatus();
            }
        }

        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            private readonly GdTask<T?> _task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in GdTask<T?> task)
            {
                this._task = task;
            }

            public bool IsCompleted
            {
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return _task.Status.IsCompleted();
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T? GetResult()
            {
                var s = _task.source;
                if (s == null)
                {
                    return _task.result;
                }
                else
                {
                    return s.GetResult(_task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                var s = _task.source;
                if (s == null)
                {
                    continuation();
                }
                else
                {
                    s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, _task.token);
                }
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation)
            {
                var s = _task.source;
                if (s == null)
                {
                    continuation();
                }
                else
                {
                    s.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, _task.token);
                }
            }

            /// <summary>
            /// If register manually continuation, you can use it instead of for compiler OnCompleted methods.
            /// </summary>
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SourceOnCompleted(Action<object> continuation, object state)
            {
                var s = _task.source;
                if (s == null)
                {
                    continuation(state);
                }
                else
                {
                    s.OnCompleted(continuation, state, _task.token);
                }
            }
        }
    }
}