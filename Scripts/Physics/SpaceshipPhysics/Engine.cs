using Godot;
using System;

public partial class Engine : GpuParticles2D
{
    // 四个状态待机熄灭开启加速枚举
    public enum EngineState
    {
        Idle,      // 待机
        Off,       // 熄灭
        On,        // 开启
        Boosting   // 加速
    }

    private EngineState currentState = EngineState.Idle;
    // 按下空格键轮流切换四个词条
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Space)
        {
            currentState = (EngineState)(((int)currentState + 1) % Enum.GetValues(typeof(EngineState)).Length);

        }
    }
    public override void _Ready()
    {
        // 初始化引擎
        GD.Print("Engine is ready.");
        // 设置初始状态为待机
        SetEngineState(EngineState.Idle);
    }
    public override void _PhysicsProcess(double delta)
    {
        // 根据当前状态更新引擎
        SetEngineState(currentState);
    }
    public void SetEngineState(EngineState state)
    {
        currentState = state; // 更新当前状态
        switch (state)
        {
            case EngineState.Idle:
                Emitting = true;
                Lifetime = 0.2; // 设置一个较短的生命周期表示待机
                break;
            case EngineState.Off:
                Emitting = false; // 正确的关闭方式
                break;
            case EngineState.On:
                Emitting = true;
                Lifetime = 0.4; // 正常运行
                break;
            case EngineState.Boosting:
                Emitting = true;
                Lifetime = 0.8; // 加速状态
                break;
        }
    }



}
