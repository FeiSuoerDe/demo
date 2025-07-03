using System;
using System.Threading;

namespace demo.addons.GDTask
{
    public partial class AsyncLazy
    {
        private static Action<object> _continuation = SetCompletionSource;

        private Func<GDTask> _taskFactory;
        private GdTaskCompletionSource _completionSource;
        private GDTask.Awaiter _awaiter;

        private object _syncLock;
        private bool _initialized;

        public AsyncLazy(Func<GDTask> taskFactory)
        {
            this._taskFactory = taskFactory;
            this._completionSource = new GdTaskCompletionSource();
            this._syncLock = new object();
            this._initialized = false;
        }

        internal AsyncLazy(GDTask task)
        {
            this._taskFactory = null;
            this._completionSource = new GdTaskCompletionSource();
            this._syncLock = null;
            this._initialized = true;

            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this._awaiter = awaiter;
                awaiter.SourceOnCompleted(_continuation, this);
            }
        }

        public GDTask Task
        {
            get
            {
                EnsureInitialized();
                return _completionSource.Task;
            }
        }


        public GDTask.Awaiter GetAwaiter() => Task.GetAwaiter();

        private void EnsureInitialized()
        {
            if (Volatile.Read(ref _initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        private void EnsureInitializedCore()
        {
            lock (_syncLock)
            {
                if (!Volatile.Read(ref _initialized))
                {
                    var f = Interlocked.Exchange(ref _taskFactory, null);
                    if (f != null)
                    {
                        var task = f();
                        var awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this._awaiter = awaiter;
                            awaiter.SourceOnCompleted(_continuation, this);
                        }

                        Volatile.Write(ref _initialized, true);
                    }
                }
            }
        }

        private void SetCompletionSource(in GDTask.Awaiter awaiter)
        {
            try
            {
                awaiter.GetResult();
                _completionSource.TrySetResult();
            }
            catch (Exception? ex)
            {
                _completionSource.TrySetException(ex);
            }
        }

        private static void SetCompletionSource(object state)
        {
            var self = (AsyncLazy)state;
            try
            {
                self._awaiter.GetResult();
                self._completionSource.TrySetResult();
            }
            catch (Exception? ex)
            {
                self._completionSource.TrySetException(ex);
            }
            finally
            {
                self._awaiter = default;
            }
        }
    }

    public partial class AsyncLazy<T>
    {
        private static Action<object> _continuation = SetCompletionSource;

        private Func<GdTask<T>> _taskFactory;
        private GdTaskCompletionSource<T?> _completionSource;
        private GdTask<T?>.Awaiter _awaiter;

        private object _syncLock;
        private bool _initialized;

        public AsyncLazy(Func<GdTask<T>> taskFactory)
        {
            this._taskFactory = taskFactory;
            this._completionSource = new GdTaskCompletionSource<T?>();
            this._syncLock = new object();
            this._initialized = false;
        }

        internal AsyncLazy(GdTask<T?> task)
        {
            this._taskFactory = null;
            this._completionSource = new GdTaskCompletionSource<T?>();
            this._syncLock = null;
            this._initialized = true;

            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this._awaiter = awaiter;
                awaiter.SourceOnCompleted(_continuation, this);
            }
        }

        public GdTask<T?> Task
        {
            get
            {
                EnsureInitialized();
                return _completionSource.Task;
            }
        }


        public GdTask<T?>.Awaiter GetAwaiter() => Task.GetAwaiter();

        private void EnsureInitialized()
        {
            if (Volatile.Read(ref _initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        private void EnsureInitializedCore()
        {
            lock (_syncLock)
            {
                if (!Volatile.Read(ref _initialized))
                {
                    var f = Interlocked.Exchange(ref _taskFactory, null);
                    if (f != null)
                    {
                        GdTask<T?> task = f();
                        var awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this._awaiter = awaiter;
                            awaiter.SourceOnCompleted(_continuation, this);
                        }

                        Volatile.Write(ref _initialized, true);
                    }
                }
            }
        }

        private void SetCompletionSource(in GdTask<T?>.Awaiter awaiter)
        {
            try
            {
                var result = awaiter.GetResult();
                _completionSource.TrySetResult(result);
            }
            catch (Exception? ex)
            {
                _completionSource.TrySetException(ex);
            }
        }

        private static void SetCompletionSource(object state)
        {
            var self = (AsyncLazy<T?>)state;
            try
            {
                var result = self._awaiter.GetResult();
                self._completionSource.TrySetResult(result);
            }
            catch (Exception? ex)
            {
                self._completionSource.TrySetException(ex);
            }
            finally
            {
                self._awaiter = default;
            }
        }
    }
}
