using System;
using System.Threading;
using demo.addons.GDTask.Internal;
using Godot;

namespace demo.addons.GDTask.Triggers
{
    public abstract partial class AsyncTriggerBase<T> : Node
    {
        private TriggerEvent<T?> _triggerEvent;

        internal protected bool CalledEnterTree;
        internal protected bool CalledDestroy;

        public override void _EnterTree()
        {
            CalledEnterTree = true;
        }

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
                OnDestroy();
        }

        private void OnDestroy()
        {
            if (CalledDestroy) return;
            CalledDestroy = true;

            _triggerEvent.SetCompleted();
        }

        internal void AddHandler(ITriggerHandler<T?>? handler)
        {
            _triggerEvent.Add(handler);
        }

        internal void RemoveHandler(ITriggerHandler<T?> handler)
        {
            _triggerEvent.Remove(handler);
        }

        protected void RaiseEvent(T? value)
        {
            _triggerEvent.SetResult(value);
        }
    }

    public interface IAsyncOneShotTrigger
    {
        GDTask OneShotAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOneShotTrigger
    {
        GDTask IAsyncOneShotTrigger.OneShotAsync()
        {
            _core.Reset();
            return new GDTask(this, _core.Version);
        }
    }

    public sealed partial class AsyncTriggerHandler<T> : IGdTaskSource<T>, ITriggerHandler<T>, IDisposable
    {
        private static readonly Action<object>? _cancellationCallback = CancellationCallback;

        private readonly AsyncTriggerBase<T?>? _trigger;

        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenRegistration _registration;
        private bool _isDisposed;
        private readonly bool _callOnce;

        private GdTaskCompletionSourceCore<T?> _core;

        internal CancellationToken CancellationToken => _cancellationToken;

        ITriggerHandler<T>? ITriggerHandler<T>.Prev { get; set; }
        ITriggerHandler<T>? ITriggerHandler<T>.Next { get; set; }

        internal AsyncTriggerHandler(AsyncTriggerBase<T?>? trigger, bool callOnce)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _isDisposed = true;
                return;
            }

            _trigger = trigger;
            _cancellationToken = default;
            _registration = default;
            _callOnce = callOnce;

            trigger?.AddHandler(this);

            TaskTracker.TrackActiveTask(this, 3);
        }

        internal AsyncTriggerHandler(AsyncTriggerBase<T?>? trigger, CancellationToken cancellationToken, bool callOnce)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _isDisposed = true;
                return;
            }

            _trigger = trigger;
            _cancellationToken = cancellationToken;
            _callOnce = callOnce;

            trigger?.AddHandler(this);

            if (cancellationToken.CanBeCanceled)
            {
                _registration = cancellationToken.RegisterWithoutCaptureExecutionContext(_cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        private static void CancellationCallback(object state)
        {
            var self = (AsyncTriggerHandler<T?>)state;
            self.Dispose();

            self._core.TrySetCanceled(self._cancellationToken);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                TaskTracker.RemoveTracking(this);
                _registration.Dispose();
                _trigger?.RemoveHandler(this);
            }
        }

        T? IGdTaskSource<T>.GetResult(short token)
        {
            try
            {
                return _core.GetResult(token);
            }
            finally
            {
                if (_callOnce)
                {
                    Dispose();
                }
            }
        }

        void ITriggerHandler<T>.OnNext(T? value)
        {
            _core.TrySetResult(value);
        }

        void ITriggerHandler<T>.OnCanceled(CancellationToken cancellationToken)
        {
            _core.TrySetCanceled(cancellationToken);
        }

        void ITriggerHandler<T>.OnCompleted()
        {
            _core.TrySetCanceled(CancellationToken.None);
        }

        void ITriggerHandler<T>.OnError(Exception? ex)
        {
            _core.TrySetException(ex);
        }

        void IGdTaskSource.GetResult(short token)
        {
            ((IGdTaskSource<T?>)this).GetResult(token);
        }

        GdTaskStatus IGdTaskSource.GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        GdTaskStatus IGdTaskSource.UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        void IGdTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            _core.OnCompleted(continuation, state, token);
        }
    }
}