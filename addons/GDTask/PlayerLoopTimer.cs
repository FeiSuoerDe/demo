using System;
using System.Threading;
using demo.addons.GDTask.Autoload;
using demo.addons.GDTask.Internal;
using Godot;

namespace demo.addons.GDTask
{
    public abstract class PlayerLoopTimer : IDisposable, IPlayerLoopItem
    {
        private readonly CancellationToken _cancellationToken;
        private readonly Action<object>? _timerCallback;
        private readonly object _state;
        private readonly PlayerLoopTiming _playerLoopTiming;
        private readonly bool _periodic;

        private bool _isRunning;
        private bool _tryStop;
        private bool _isDisposed;

        protected PlayerLoopTimer(bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object>? timerCallback, object state)
        {
            this._periodic = periodic;
            this._playerLoopTiming = playerLoopTiming;
            this._cancellationToken = cancellationToken;
            this._timerCallback = timerCallback;
            this._state = state;
        }

        public static PlayerLoopTimer Create(TimeSpan interval, bool periodic, DelayType delayType, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object>? timerCallback, object state)
        {
#if DEBUG
            // force use Realtime.
            if (GDTaskPlayerLoopAutoload.IsMainThread && Engine.IsEditorHint())
            {
                delayType = DelayType.Realtime;
            }
#endif

            switch (delayType)
            {
                case DelayType.Realtime:
                    return new RealtimePlayerLoopTimer(interval, periodic, playerLoopTiming, cancellationToken, timerCallback, state);
                case DelayType.DeltaTime:
                default:
                    return new DeltaTimePlayerLoopTimer(interval, periodic, playerLoopTiming, cancellationToken, timerCallback, state);
            }
        }

        public static PlayerLoopTimer StartNew(TimeSpan interval, bool periodic, DelayType delayType, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object>? timerCallback, object state)
        {
            var timer = Create(interval, periodic, delayType, playerLoopTiming, cancellationToken, timerCallback, state);
            timer.Restart();
            return timer;
        }

        /// <summary>
        /// Restart(Reset and Start) timer.
        /// </summary>
        public void Restart()
        {
            if (_isDisposed) throw new ObjectDisposedException(null);

            ResetCore(null); // init state
            if (!_isRunning)
            {
                _isRunning = true;
                GDTaskPlayerLoopAutoload.AddAction(_playerLoopTiming, this);
            }
            _tryStop = false;
        }

        /// <summary>
        /// Restart(Reset and Start) and change interval.
        /// </summary>
        public void Restart(TimeSpan interval)
        {
            if (_isDisposed) throw new ObjectDisposedException(null);

            ResetCore(interval); // init state
            if (!_isRunning)
            {
                _isRunning = true;
                GDTaskPlayerLoopAutoload.AddAction(_playerLoopTiming, this);
            }
            _tryStop = false;
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public void Stop()
        {
            _tryStop = true;
        }

        protected abstract void ResetCore(TimeSpan? newInterval);

        public void Dispose()
        {
            _isDisposed = true;
        }

        bool IPlayerLoopItem.MoveNext()
        {
            if (_isDisposed)
            {
                _isRunning = false;
                return false;
            }
            if (_tryStop)
            {
                _isRunning = false;
                return false;
            }
            if (_cancellationToken.IsCancellationRequested)
            {
                _isRunning = false;
                return false;
            }

            if (!MoveNextCore())
            {
                _timerCallback(_state);

                if (_periodic)
                {
                    ResetCore(null);
                    return true;
                }
                else
                {
                    _isRunning = false;
                    return false;
                }
            }

            return true;
        }

        protected abstract bool MoveNextCore();
    }

    internal sealed class DeltaTimePlayerLoopTimer : PlayerLoopTimer
    {
        private bool _isMainThread;
        private ulong _initialFrame;
        private double _elapsed;
        private double _interval;

        public DeltaTimePlayerLoopTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object>? timerCallback, object state)
            : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
        {
            ResetCore(interval);
        }

        protected override bool MoveNextCore()
        {
            if (_elapsed == 0.0)
            {
                if (_isMainThread && _initialFrame == Engine.GetProcessFrames())
                {
                    return true;
                }
            }

            _elapsed += GDTaskPlayerLoopAutoload.Global.DeltaTime;
            if (_elapsed >= _interval)
            {
                return false;
            }

            return true;
        }

        protected override void ResetCore(TimeSpan? interval)
        {
            this._elapsed = 0.0;
            this._isMainThread = GDTaskPlayerLoopAutoload.IsMainThread;
            if (this._isMainThread)
                this._initialFrame = Engine.GetProcessFrames();
            if (interval != null)
            {
                this._interval = (float)interval.Value.TotalSeconds;
            }
        }
    }

    internal sealed class RealtimePlayerLoopTimer : PlayerLoopTimer
    {
        private ValueStopwatch _stopwatch;
        private long _intervalTicks;

        public RealtimePlayerLoopTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object>? timerCallback, object state)
            : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
        {
            ResetCore(interval);
        }

        protected override bool MoveNextCore()
        {
            if (_stopwatch.ElapsedTicks >= _intervalTicks)
            {
                return false;
            }

            return true;
        }

        protected override void ResetCore(TimeSpan? interval)
        {
            this._stopwatch = ValueStopwatch.StartNew();
            if (interval != null)
            {
                this._intervalTicks = interval.Value.Ticks;
            }
        }
    }
}

