#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace demo.addons.R3.Godot;

public class GodotTimeProvider : TimeProvider
{
    public static readonly GodotTimeProvider Process = new GodotTimeProvider(GodotFrameProvider.Process);
    public static readonly GodotTimeProvider PhysicsProcess = new GodotTimeProvider(GodotFrameProvider.PhysicsProcess);

    private readonly GodotFrameProvider _frameProvider;

    internal double Time;

    private GodotTimeProvider(FrameProvider frameProvider)
    {
        this._frameProvider = (GodotFrameProvider)frameProvider;
    }

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        return new FrameTimer(callback, state, dueTime, period, _frameProvider);
    }

    public override long GetTimestamp()
    {
        return TimeSpan.FromSeconds(Time).Ticks;
    }
}

internal sealed class FrameTimer : ITimer, IFrameRunnerWorkItem
{
    private enum RunningState
    {
        Stop,
        RunningDueTime,
        RunningPeriod,
        ChangeRequested
    }

    private readonly TimerCallback _callback;
    private readonly object? _state;
    private readonly GodotFrameProvider _frameProvider;
    private readonly object _gate = new object();

    private TimeSpan _dueTime;
    private TimeSpan _period;
    private RunningState _runningState;
    private double _elapsed;
    private bool _isDisposed;

    public FrameTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period, GodotFrameProvider frameProvider)
    {
        this._callback = callback;
        this._state = state;
        this._dueTime = dueTime;
        this._period = period;
        this._frameProvider = frameProvider;
        Change(dueTime, period);
    }

    public bool Change(TimeSpan dueTime, TimeSpan period)
    {
        if (_isDisposed) return false;

        lock (_gate)
        {
            this._dueTime = dueTime;
            this._period = period;

            if (dueTime == Timeout.InfiniteTimeSpan)
            {
                if (_runningState == RunningState.Stop)
                {
                    return true;
                }
            }

            if (_runningState == RunningState.Stop)
            {
                _frameProvider.Register(this);
            }

            _runningState = RunningState.ChangeRequested;
        }
        return true;
    }

    bool IFrameRunnerWorkItem.MoveNext(long frameCount)
    {
        if (_isDisposed) return false;

        RunningState runState;
        TimeSpan p; // period
        TimeSpan d; // dueTime
        lock (_gate)
        {
            runState = _runningState;

            if (runState == RunningState.ChangeRequested)
            {
                _elapsed = 0;
                if (_dueTime == Timeout.InfiniteTimeSpan)
                {
                    _runningState = RunningState.Stop;
                    return false;
                }

                runState = _runningState = RunningState.RunningDueTime;
            }
            p = _period;
            d = _dueTime;
        }

        _elapsed += _frameProvider.Delta.Value;

        try
        {
            if (runState == RunningState.RunningDueTime)
            {
                var dt = (double)d.TotalSeconds;
                if (_elapsed >= dt)
                {
                    _callback(_state);

                    _elapsed = 0;
                    if (_period == Timeout.InfiniteTimeSpan)
                    {
                        return ChangeState(RunningState.Stop);
                    }
                    else
                    {
                        return ChangeState(RunningState.RunningPeriod);
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var dt = (double)p.TotalSeconds;
                if (_elapsed >= dt)
                {
                    _callback(_state);
                    _elapsed = 0;
                }

                return ChangeState(RunningState.RunningPeriod);
            }
        }
        catch (Exception ex)
        {
            ObservableSystem.GetUnhandledExceptionHandler().Invoke(ex);
            return ChangeState(RunningState.Stop);
        }
    }

    private bool ChangeState(RunningState state)
    {
        lock (_gate)
        {
            // change requested is high priority
            if (_runningState == RunningState.ChangeRequested)
            {
                return true;
            }

            switch (state)
            {
                case RunningState.RunningPeriod:
                    _runningState = state;
                    return true;
                default: // otherwise(Stop)
                    _runningState = state;
                    return false;
            }
        }
    }

    public void Dispose()
    {
        Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _isDisposed = true;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }
}
