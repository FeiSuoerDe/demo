using Godot;
using System;

public partial class MapContShip : RigidBody2D
{
    [Export]
    public float ThrustForce = 1000.0f; // 推进力大小
    [Export]
    public float DampingFactor = 0.98f; // 阻尼系数，控制惯性衰减
    [Export]
    public float MaxSpeed = 500.0f; // 最大速度

    public override void _Ready()
    {
        // 初始化飞船位置
        Position = new Vector2(2500, 2500);

        // 设置物理属性
        LinearDamp = 1.0f; // 线性阻尼
        Mass = 1.0f; // 质量
    }

    public override void _PhysicsProcess(double delta)
    {
        // 获取鼠标位置
        Vector2 mousePos = GetGlobalMousePosition();

        // 计算朝向鼠标的方向向量
        Vector2 direction = (mousePos - Position).Normalized();

        // 当按下鼠标左键时施加推力
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            // 施加推力
            ApplyForce(direction * ThrustForce);
        }

        // 应用速度阻尼
        LinearVelocity *= DampingFactor;

        // 限制最大速度
        if (LinearVelocity.Length() > MaxSpeed)
        {
            LinearVelocity = LinearVelocity.Normalized() * MaxSpeed;
        }
    }
}
