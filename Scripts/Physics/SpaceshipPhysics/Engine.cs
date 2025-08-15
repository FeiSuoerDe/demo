using Godot;

namespace TimelapseInvoices.Scripts.Physics.SpaceshipPhysics;

public class EngineData

{
    // 在飞船贴图的相对位置

    public Vector2 ToPosition { get; set; } = new Vector2(0, 0);
    // 四个状态待机熄灭开启加速枚
    public enum EngineState
    {
        Idle,      // 待机
        Off,       // 熄灭
        On,        // 开启
        Boosting   // 加速
    }

    public EngineState CurrentState { get; set; } = EngineState.Idle;

    // 添加引擎参数配置
    public float IdleLifetime { get; set; } = 0.2f;
    public float OnLifetime { get; set; } = 0.4f;
    public float BoostingLifetime { get; set; } = 0.8f;

    public EngineData()
    {
        // 默认构造函数
    }

    public EngineData(EngineState initialState)
    {
        CurrentState = initialState;
    }

    // 切换到下一个状态
    public void CycleState()
    {
        CurrentState = (EngineState)(((int)CurrentState + 1) % System.Enum.GetValues(typeof(EngineState)).Length);
    }

    // 获取当前状态的生命周期
    public float GetCurrentLifetime()
    {
        switch (CurrentState)
        {
            case EngineState.Idle:
                return IdleLifetime;
            case EngineState.On:
                return OnLifetime;
            case EngineState.Boosting:
                return BoostingLifetime;
            default:
                return 0f; // Off state
        }
    }

    // 克隆数据
    public EngineData Clone()
    {
        return new EngineData(CurrentState)
        {
            IdleLifetime = this.IdleLifetime,
            OnLifetime = this.OnLifetime,
            BoostingLifetime = this.BoostingLifetime
        };
    }
}
[GlobalClass]

public partial class Engine : GpuParticles2D
{
    // 数据对象
    public EngineData Data = new EngineData();

    public override void _Ready()
    {
        // 初始化引擎
        GD.Print("引擎开机");
        // 设置初始状态为待机
        SetEngineState(EngineData.EngineState.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        // 根据当前状态更新引擎
        SetEngineState(Data.CurrentState);
    }

    public void SetEngineState(EngineData.EngineState state)
    {
        Data.CurrentState = state; // 更新当前状态
        switch (state)
        {
            case EngineData.EngineState.Idle:
                Emitting = true;
                Lifetime = Data.IdleLifetime;
                break;
            case EngineData.EngineState.Off:
                Emitting = false; // 正确的关闭方式
                break;
            case EngineData.EngineState.On:
                Emitting = true;
                Lifetime = Data.OnLifetime;
                break;
            case EngineData.EngineState.Boosting:
                Emitting = true;
                Lifetime = Data.BoostingLifetime;
                break;
        }
    }

    // 切换到下一个引擎状态
    public void CycleEngineState()
    {
        Data.CycleState();
        SetEngineState(Data.CurrentState);
    }

    // 设置引擎参数
    public void ConfigureEngine(float idleLifetime, float onLifetime, float boostingLifetime)
    {
        Data.IdleLifetime = idleLifetime;
        Data.OnLifetime = onLifetime;
        Data.BoostingLifetime = boostingLifetime;

        // 更新当前状态以应用新设置
        SetEngineState(Data.CurrentState);
    }
}
