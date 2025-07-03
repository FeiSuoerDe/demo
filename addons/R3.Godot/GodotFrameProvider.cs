#nullable enable

using System;
using System.Runtime.CompilerServices;
using Godot;
using R3;
using R3.Collections;

namespace demo.addons.R3.Godot;

internal enum PlayerLoopTiming
{
    Process,
    PhysicsProcess
}

public class GodotFrameProvider : FrameProvider
{
    public static readonly GodotFrameProvider Process = new GodotFrameProvider(PlayerLoopTiming.Process);
    public static readonly GodotFrameProvider PhysicsProcess = new GodotFrameProvider(PlayerLoopTiming.PhysicsProcess);

    private FreeListCore<IFrameRunnerWorkItem> _list;
    private readonly object _gate = new object();

    private PlayerLoopTiming PlayerLoopTiming { get; }

    internal StrongBox<double> Delta = default!; // set from Node before running process.

    internal GodotFrameProvider(PlayerLoopTiming playerLoopTiming)
    {
        this.PlayerLoopTiming = playerLoopTiming;
        this._list = new FreeListCore<IFrameRunnerWorkItem>(_gate);
    }

    public override long GetFrameCount()
    {
        if (PlayerLoopTiming == PlayerLoopTiming.Process)
        {
            return (long)Engine.GetProcessFrames();
        }
        else
        {
            return (long)Engine.GetPhysicsFrames();
        }
    }

    public override void Register(IFrameRunnerWorkItem callback)
    {
        _list.Add(callback, out _);
    }

    internal void Run(double _)
    {
        long frameCount = GetFrameCount();

        var span = _list.AsSpan();
        for (int i = 0; i < span.Length; i++)
        {
            ref readonly var item = ref span[i];
            if (item != null)
            {
                try
                {
                    if (!item.MoveNext(frameCount))
                    {
                        _list.Remove(i);
                    }
                }
                catch (Exception ex)
                {
                    _list.Remove(i);
                    try
                    {
                        ObservableSystem.GetUnhandledExceptionHandler().Invoke(ex);
                    }
                    catch { }
                }
            }
        }
    }
}
