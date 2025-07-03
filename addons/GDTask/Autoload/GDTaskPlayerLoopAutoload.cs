using System;
using demo.addons.GDTask.Internal;
using Godot;

namespace demo.addons.GDTask.Autoload;

public enum PlayerLoopTiming
{
    Process = 0,
    PhysicsProcess = 1,
    PauseProcess = 2,
    PausePhysicsProcess = 3,
}

public interface IPlayerLoopItem
{
    bool MoveNext();
}

/// <summary>
/// Singletons that forwards Godot calls and values to GDTasks.
/// </summary>
public partial class GDTaskPlayerLoopAutoload : Node
{
    public static int MainThreadId => Global._mainThreadId;

    public static bool IsMainThread => System.Environment.CurrentManagedThreadId == Global._mainThreadId;
    public static void AddAction(PlayerLoopTiming timing, IPlayerLoopItem action) => Global.LocalAddAction(timing, action);
    private static void ThrowInvalidLoopTiming(PlayerLoopTiming playerLoopTiming) => throw new InvalidOperationException("Target playerLoopTiming is not injected. Please check PlayerLoopHelper.Initialize. PlayerLoopTiming:" + playerLoopTiming);
    public static void AddContinuation(PlayerLoopTiming timing, Action continuation) => Global.LocalAddContinuation(timing, continuation);

    private void LocalAddAction(PlayerLoopTiming timing, IPlayerLoopItem action)
    {
        var runner = _runners?[(int)timing];
        if (runner == null)
        {
            ThrowInvalidLoopTiming(timing);
        }
        runner?.AddAction(action);
    }

    // NOTE: Continuation means a asynchronous task invoked by another task after the other task finishes.
    private void LocalAddContinuation(PlayerLoopTiming timing, Action continuation)
    {
        var q = _yielders?[(int)timing];
        q?.Enqueue(continuation);
    }

    public static GDTaskPlayerLoopAutoload Global
    {
        get
        {
            if (_sGlobal != null) return _sGlobal;

            var newInstance = new GDTaskPlayerLoopAutoload();
            newInstance.Initialize();
            var currentScene = ((SceneTree)Engine.GetMainLoop()).CurrentScene;
            currentScene.AddChild(newInstance);
            currentScene.MoveChild(newInstance, 0);
            newInstance.Name = "GDTaskPlayerLoopAutoload";
            _sGlobal = newInstance;

            return _sGlobal;
        }
    }
    public double DeltaTime => GetProcessDeltaTime();
    public double PhysicsDeltaTime => GetPhysicsProcessDeltaTime();

    private static GDTaskPlayerLoopAutoload? _sGlobal;
    private int _mainThreadId;
    private ContinuationQueue[]? _yielders;
    private PlayerLoopRunner[]? _runners;
    private ProcessListener? _processListener;

    public override void _EnterTree()
    {
        if (_sGlobal == null)
        {
            Initialize();
            _sGlobal = this;
            return;
        }
        QueueFree();
    }

    private void Initialize()
    {
        ProcessMode = ProcessModeEnum.Pausable;
        _mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        _yielders =
        [
            new ContinuationQueue(PlayerLoopTiming.Process),
            new ContinuationQueue(PlayerLoopTiming.PhysicsProcess),
            new ContinuationQueue(PlayerLoopTiming.PauseProcess),
            new ContinuationQueue(PlayerLoopTiming.PausePhysicsProcess)
        ];
        _runners =
        [
            new PlayerLoopRunner(PlayerLoopTiming.Process),
            new PlayerLoopRunner(PlayerLoopTiming.PhysicsProcess),
            new PlayerLoopRunner(PlayerLoopTiming.PauseProcess),
            new PlayerLoopRunner(PlayerLoopTiming.PausePhysicsProcess)
        ];
        _processListener = new ProcessListener();
        AddChild(_processListener);
        _processListener.ProcessMode = ProcessModeEnum.Always;
        _processListener.OnProcess += PauseProcess;
        _processListener.OnPhysicsProcess += PausePhysicsProcess;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationPredelete) return;
        if (Global == this)
            _sGlobal = null;
        if (_yielders == null) return;
        foreach (var yielder in _yielders)
            yielder.Clear();
        if (_runners == null) return;
        foreach (var runner in _runners)
            runner.Clear();
    }

    public override void _Process(double delta)
    {
        _yielders?[(int)PlayerLoopTiming.Process].Run();
        _runners?[(int)PlayerLoopTiming.Process].Run();
    }

    public override void _PhysicsProcess(double delta)
    {
        _yielders?[(int)PlayerLoopTiming.PhysicsProcess].Run();
        _runners?[(int)PlayerLoopTiming.PhysicsProcess].Run();
    }

    private void PauseProcess(double delta)
    {
        _yielders?[(int)PlayerLoopTiming.PauseProcess].Run();
        _runners?[(int)PlayerLoopTiming.PauseProcess].Run();
    }

    private void PausePhysicsProcess(double delta)
    {
        _yielders?[(int)PlayerLoopTiming.PausePhysicsProcess].Run();
        _runners?[(int)PlayerLoopTiming.PausePhysicsProcess].Run();
    }
}