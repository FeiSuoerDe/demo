using System;
using System.Collections.Generic;
using System.Threading;
using demo.addons.GDTask.Internal;

namespace demo.addons.GDTask
{
    public partial struct GDTask
    {
        public static GdTask<T[]?> WhenAll<T>(params GdTask<T>[] tasks)
        {
            if (tasks.Length == 0)
            {
                return GDTask.FromResult(Array.Empty<T>());
            }

            return new GdTask<T[]?>(new WhenAllPromise<T?>(tasks, tasks.Length), 0);
        }

        public static GdTask<T[]> WhenAll<T>(IEnumerable<GdTask<T>> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                var promise = new WhenAllPromise<T?>(span.Array, span.Length); // consumed array in constructor.
                return new GdTask<T[]>(promise, 0);
            }
        }

        public static GDTask WhenAll(params GDTask[] tasks)
        {
            if (tasks.Length == 0)
            {
                return GDTask.CompletedTask;
            }

            return new GDTask(new WhenAllPromise(tasks, tasks.Length), 0);
        }

        public static GDTask WhenAll(IEnumerable<GDTask> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                var promise = new WhenAllPromise(span.Array, span.Length); // consumed array in constructor.
                return new GDTask(promise, 0);
            }
        }

        private sealed class WhenAllPromise<T> : IGdTaskSource<T[]>
        {
            private T?[] _result;
            private int _completeCount;
            private GdTaskCompletionSourceCore<T?[]> _core; // don't reset(called after GetResult, will invoke TrySetException.)

            public WhenAllPromise(GdTask<T?>[] tasks, int tasksLength)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this._completeCount = 0;

                if (tasksLength == 0)
                {
                    this._result = Array.Empty<T>();
                    _core.TrySetResult(_result);
                    return;
                }

                this._result = new T[tasksLength];

                for (int i = 0; i < tasksLength; i++)
                {
                    GdTask<T?>.Awaiter awaiter;
                    try
                    {
                        awaiter = tasks[i].GetAwaiter();
                    }
                    catch (Exception? ex)
                    {
                        _core.TrySetException(ex);
                        continue;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter, i);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T>, GdTask<T?>.Awaiter, int>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2, t.Item3);
                            }
                        }, StateTuple.Create(this, awaiter, i));
                    }
                }
            }

            private static void TryInvokeContinuation(WhenAllPromise<T> self, in GdTask<T?>.Awaiter awaiter, int i)
            {
                try
                {
                    self._result[i] = awaiter.GetResult();
                }
                catch (Exception? ex)
                {
                    self._core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self._completeCount) == self._result.Length)
                {
                    self._core.TrySetResult(self._result);
                }
            }

            public T?[] GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return _core.GetResult(token);
            }

            void IGdTaskSource.GetResult(short token)
            {
                GetResult(token);
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
        }

        private sealed class WhenAllPromise : IGdTaskSource
        {
            private int _completeCount;
            private int _tasksLength;
            private GdTaskCompletionSourceCore<AsyncUnit> _core; // don't reset(called after GetResult, will invoke TrySetException.)

            public WhenAllPromise(GDTask[] tasks, int tasksLength)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this._tasksLength = tasksLength;
                this._completeCount = 0;

                if (tasksLength == 0)
                {
                    _core.TrySetResult(AsyncUnit.Default);
                    return;
                }

                for (int i = 0; i < tasksLength; i++)
                {
                    GDTask.Awaiter awaiter;
                    try
                    {
                        awaiter = tasks[i].GetAwaiter();
                    }
                    catch (Exception? ex)
                    {
                        _core.TrySetException(ex);
                        continue;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise, GDTask.Awaiter>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            private static void TryInvokeContinuation(WhenAllPromise self, in GDTask.Awaiter awaiter)
            {
                try
                {
                    awaiter.GetResult();
                }
                catch (Exception? ex)
                {
                    self._core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self._completeCount) == self._tasksLength)
                {
                    self._core.TrySetResult(AsyncUnit.Default);
                }
            }

            public void GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                _core.GetResult(token);
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
        }
    }
}
