using Godot;
using System;

public partial class Rocket : RigidBody2D
{
    // 目标node2d
    public RigidBody2D Target;
    // 速度
    [Export]
    public float Speed = 500.0f; // 火箭速度，单位为像素/秒
    [Export]
    // 追踪半径
    public float TrackingRadius = 500.0f; // 追踪半径，单位为像素
    [Export]
    // 追踪area
    public Area2D TrackingArea;
    [Export]
    // 追踪coll
    public CollisionShape2D TrackingCollisionShape;

    // 惯性控制参数
    [Export]
    public float TurnRate = 15.0f; // 转向速率，较小的值会产生更大的惯性
    [Export]
    public float MaxTurnAngle = 2; // 每秒最大转向角度
    [Export]
    public float AccelerationRate = 1.5f; // 加速率

    // 当前速度向量和当前方向
    private Vector2 _currentVelocity = Vector2.Zero;
    private Vector2 _currentDirection = Vector2.Right;

    // 初始化
    public override void _Ready()
    {
        base._Ready();
        // 设置追踪区域半径
        if (TrackingArea != null)
        {
            TrackingCollisionShape.Shape = new CircleShape2D { Radius = TrackingRadius };
        }
        else
        {
            GD.PrintErr("目标呢?");
        }

        // 初始化当前方向为火箭的初始朝向
        _currentDirection = Transform.X.Normalized();
    }

    // 带惯性的前进方法
    public void MoveForward(float delta)
    {
        // 获取当前朝向作为目标方向
        Vector2 targetDirection = Transform.X.Normalized();

        // 平滑过渡到目标方向（考虑惯性）
        _currentDirection = _currentDirection.Lerp(targetDirection, TurnRate * delta).Normalized();

        // 平滑加速到目标速度
        float targetSpeed = Speed;
        _currentVelocity = _currentVelocity.Lerp(_currentDirection * targetSpeed, AccelerationRate * delta);

        // 应用速度
        LinearVelocity = _currentVelocity;

        // 使火箭的朝向匹配移动方向
        if (_currentVelocity.Length() > 10f)
        {
            float targetRotation = Mathf.Atan2(_currentVelocity.Y, _currentVelocity.X);
            Rotation = Mathf.LerpAngle(Rotation, targetRotation, TurnRate * delta);
        }
    }

    // 带惯性的追踪目标方法
    public void TrackTarget(float delta)
    {
        if (Target == null) return; // 如果没有目标，直接返回

        // 计算目标方向
        Vector2 toTarget = (Target.Position - Position);
        Vector2 targetDirection = toTarget.Normalized();

        // 平滑过渡到目标方向（考虑惯性）
        _currentDirection = _currentDirection.Lerp(targetDirection, TurnRate * delta).Normalized();

        // 平滑加速到目标速度
        float targetSpeed = Speed;
        _currentVelocity = _currentVelocity.Lerp(_currentDirection * targetSpeed, AccelerationRate * delta);

        // 应用速度
        LinearVelocity = _currentVelocity;

        // 使火箭的朝向匹配移动方向
        if (_currentVelocity.Length() > 10f)
        {
            float targetRotation = Mathf.Atan2(_currentVelocity.Y, _currentVelocity.X);
            Rotation = Mathf.LerpAngle(Rotation, targetRotation, TurnRate * delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // 不调用基类的物理处理，因为我们自己处理移动
        // base._PhysicsProcess(delta);

        if (Target != null)
        {
            GD.Print($"追踪目标: {Target.Name}");
            TrackTarget((float)delta);
        }
        else
        {
            MoveForward((float)delta);
        }
    }

    // 信号
    public void _on_area_2d_body_entered(Node body)
    {
        if (body is RigidBody2D rigidBody)
        {
            Target = rigidBody; // 设置目标为进入的刚体
            GD.Print($"目标已设置: {Target.Name}");
        }
    }
}
