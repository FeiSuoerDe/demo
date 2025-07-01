using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using demo.addons.GDTask.Autoload;
using demo.addons.GDTask.Internal;
using Godot;

namespace demo.addons.GDTask
{
    public partial struct GDTask
    {
        public static GDTask WaitUntil(Func<bool>? predicate, PlayerLoopTiming timing = PlayerLoopTiming.Process, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new GDTask(WaitUntilPromise.Create(predicate, timing, cancellationToken, out var token), token);
        }

        public static GDTask WaitWhile(Func<bool>? predicate, PlayerLoopTiming timing = PlayerLoopTiming.Process, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new GDTask(WaitWhilePromise.Create(predicate, timing, cancellationToken, out var token), token);
        }

        public static GDTask WaitUntilCanceled(CancellationToken cancellationToken, PlayerLoopTiming timing = PlayerLoopTiming.Process)
        {
            return new GDTask(WaitUntilCanceledPromise.Create(cancellationToken, timing, out var token), token);
        }

        public static GdTask<TU?> WaitUntilValueChanged<T, TU>(T? target, Func<T?, TU?>? monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Process, IEqualityComparer<TU>? equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken))
          where T : class
        {
            var isGodotObject = target is GodotObject; // don't use (unityObject == null)

            Debug.Assert(target != null, nameof(target) + " != null");
            Debug.Assert(monitorFunction != null, nameof(monitorFunction) + " != null");
            Debug.Assert(equalityComparer != null, nameof(equalityComparer) + " != null");
            return new GdTask<TU?>(isGodotObject
                ? WaitUntilValueChangedGodotObjectPromise<T, TU>.Create(target, monitorFunction, equalityComparer, monitorTiming, cancellationToken, out var token)
                : WaitUntilValueChangedStandardObjectPromise<T, TU>.Create(target, monitorFunction, equalityComparer, monitorTiming, cancellationToken, out token), token);
        }

        private sealed class WaitUntilPromise : IGdTaskSource, IPlayerLoopItem, ITaskPoolNode<WaitUntilPromise>
        {
            private static TaskPool<WaitUntilPromise> _pool;
            private WaitUntilPromise? _nextNode;
            public ref WaitUntilPromise? NextNode => ref _nextNode;

            static WaitUntilPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitUntilPromise), () => _pool.Size);
            }

            private Func<bool>? _predicate;
            private CancellationToken _cancellationToken;

            private GdTaskCompletionSourceCore<object?> _core;

            private WaitUntilPromise()
            {
            }

            public static IGdTaskSource Create(Func<bool>? predicate, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetGdTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new WaitUntilPromise();
                }

                result._predicate = predicate;
                result._cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                GDTaskPlayerLoopAutoload.AddAction(timing, result);

                token = result._core.Version;
                return result;
            }

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

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return false;
                }

                try
                {
                    if (_predicate != null && !_predicate())
                    {
                        return true;
                    }
                }
                catch (Exception? ex)
                {
                    _core.TrySetException(ex);
                    return false;
                }

                _core.TrySetResult(null);
                return false;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _predicate = default;
                _cancellationToken = default;
                return _pool.TryPush(this);
            }
        }

        private sealed class WaitWhilePromise : IGdTaskSource, IPlayerLoopItem, ITaskPoolNode<WaitWhilePromise>
        {
            private static TaskPool<WaitWhilePromise> _pool;
            private WaitWhilePromise? _nextNode;
            public ref WaitWhilePromise? NextNode => ref _nextNode;

            static WaitWhilePromise()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitWhilePromise), () => _pool.Size);
            }

            private Func<bool>? _predicate;
            private CancellationToken _cancellationToken;

            private GdTaskCompletionSourceCore<object?> _core;

            private WaitWhilePromise()
            {
            }

            public static IGdTaskSource Create(Func<bool>? predicate, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetGdTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new WaitWhilePromise();
                }

                result._predicate = predicate;
                result._cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                GDTaskPlayerLoopAutoload.AddAction(timing, result);

                token = result._core.Version;
                return result;
            }

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

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return false;
                }

                try
                {
                    if (_predicate != null && _predicate())
                    {
                        return true;
                    }
                }
                catch (Exception? ex)
                {
                    _core.TrySetException(ex);
                    return false;
                }

                _core.TrySetResult(null);
                return false;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _predicate = default;
                _cancellationToken = default;
                return _pool.TryPush(this);
            }
        }

        private sealed class WaitUntilCanceledPromise : IGdTaskSource, IPlayerLoopItem, ITaskPoolNode<WaitUntilCanceledPromise>
        {
            private static TaskPool<WaitUntilCanceledPromise> _pool;
            private WaitUntilCanceledPromise? _nextNode;
            public ref WaitUntilCanceledPromise? NextNode => ref _nextNode;

            static WaitUntilCanceledPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitUntilCanceledPromise), () => _pool.Size);
            }

            private CancellationToken _cancellationToken;

            private GdTaskCompletionSourceCore<object?> _core;

            private WaitUntilCanceledPromise()
            {
            }

            public static IGdTaskSource Create(CancellationToken cancellationToken, PlayerLoopTiming timing, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetGdTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new WaitUntilCanceledPromise();
                }

                result._cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                GDTaskPlayerLoopAutoload.AddAction(timing, result);

                token = result._core.Version;
                return result;
            }

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

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _core.TrySetResult(null);
                    return false;
                }

                return true;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _cancellationToken = default;
                return _pool.TryPush(this);
            }
        }

        // where T : UnityEngine.Object, can not add constraint
        private sealed class WaitUntilValueChangedGodotObjectPromise<T, TU> : IGdTaskSource<TU>, IPlayerLoopItem, ITaskPoolNode<WaitUntilValueChangedGodotObjectPromise<T, TU>>
        {
            private static TaskPool<WaitUntilValueChangedGodotObjectPromise<T, TU>> _pool;
            private WaitUntilValueChangedGodotObjectPromise<T, TU>? _nextNode;
            public ref WaitUntilValueChangedGodotObjectPromise<T, TU>? NextNode => ref _nextNode;

            static WaitUntilValueChangedGodotObjectPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitUntilValueChangedGodotObjectPromise<T, TU>), () => _pool.Size);
            }

            private T? _target;
            private GodotObject? _targetAsGodotObject;
            private TU? _currentValue;
            private Func<T, TU?>? _monitorFunction;
            private IEqualityComparer<TU>? _equalityComparer;
            private CancellationToken _cancellationToken;

            private GdTaskCompletionSourceCore<TU?> _core;

            private WaitUntilValueChangedGodotObjectPromise()
            {
            }

            public static IGdTaskSource<TU?> Create(T? target, Func<T?, TU?>? monitorFunction, IEqualityComparer<TU?>? equalityComparer, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetGdTaskCompletionSource<TU>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new WaitUntilValueChangedGodotObjectPromise<T, TU>();
                }

                result._target = target;
                result._targetAsGodotObject = target as GodotObject;
                result._monitorFunction = monitorFunction;
                if (target != null)
                    if (monitorFunction != null)
                        result._currentValue = monitorFunction(target);
                result._equalityComparer = equalityComparer ?? GodotEqualityComparer.GetDefault<TU>();
                result._cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                GDTaskPlayerLoopAutoload.AddAction(timing, result);

                token = result._core.Version;
                return result;
            }

            public TU? GetResult(short token)
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

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested || !GodotObject.IsInstanceValid(_targetAsGodotObject)) // destroyed = cancel.
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return false;
                }

                TU? nextValue;
                try
                {
                    Debug.Assert(_monitorFunction != null, nameof(_monitorFunction) + " != null");
                    Debug.Assert(_target != null, nameof(_target) + " != null");
                    nextValue = _monitorFunction(_target);
                    if (_equalityComparer != null && _equalityComparer.Equals(_currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception? ex)
                {
                    _core.TrySetException(ex);
                    return false;
                }

                _core.TrySetResult(nextValue);
                return false;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _target = default;
                _currentValue = default;
                _monitorFunction = default;
                _equalityComparer = default;
                _cancellationToken = default;
                return _pool.TryPush(this);
            }
        }

        private sealed class WaitUntilValueChangedStandardObjectPromise<T, TU> : IGdTaskSource<TU>, IPlayerLoopItem, ITaskPoolNode<WaitUntilValueChangedStandardObjectPromise<T, TU>>
            where T : class
        {
            private static TaskPool<WaitUntilValueChangedStandardObjectPromise<T, TU>> _pool;
            private WaitUntilValueChangedStandardObjectPromise<T, TU>? _nextNode;
            public ref WaitUntilValueChangedStandardObjectPromise<T, TU>? NextNode => ref _nextNode;

            static WaitUntilValueChangedStandardObjectPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitUntilValueChangedStandardObjectPromise<T, TU>), () => _pool.Size);
            }

            private WeakReference<T>? _target;
            private TU? _currentValue;
            private Func<T, TU>? _monitorFunction;
            private IEqualityComparer<TU>? _equalityComparer;
            private CancellationToken _cancellationToken;

            private GdTaskCompletionSourceCore<TU?> _core;

            private WaitUntilValueChangedStandardObjectPromise()
            {
            }

            public static IGdTaskSource<TU?> Create(T target, Func<T, TU>? monitorFunction, IEqualityComparer<TU>? equalityComparer, PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetGdTaskCompletionSource<TU>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!_pool.TryPop(out var result))
                {
                    result = new WaitUntilValueChangedStandardObjectPromise<T, TU>();
                }

                result._target = new WeakReference<T>(target, false); // wrap in WeakReference.
                result._monitorFunction = monitorFunction;
                if (monitorFunction != null) result._currentValue = monitorFunction(target);
                result._equalityComparer = equalityComparer ?? GodotEqualityComparer.GetDefault<TU>();
                result._cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                GDTaskPlayerLoopAutoload.AddAction(timing, result);

                token = result._core.Version;
                return result;
            }

            public TU? GetResult(short token)
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

            public bool MoveNext()
            {
                Debug.Assert(_target != null, nameof(_target) + " != null");
                if (_cancellationToken.IsCancellationRequested || !_target.TryGetTarget(out var t)) // doesn't find = cancel.
                {
                    _core.TrySetCanceled(_cancellationToken);
                    return false;
                }

                TU? nextValue;
                try
                {
                    Debug.Assert(_monitorFunction != null, nameof(_monitorFunction) + " != null");
                    nextValue = _monitorFunction(t);
                    if (_equalityComparer != null && _equalityComparer.Equals(_currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception? ex)
                {
                    _core.TrySetException(ex);
                    return false;
                }

                _core.TrySetResult(nextValue);
                return false;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                _core.Reset();
                _target = default;
                _currentValue = default;
                _monitorFunction = default;
                _equalityComparer = default;
                _cancellationToken = default;
                return _pool.TryPush(this);
            }
        }
    }
}
