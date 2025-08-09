using Godot;

namespace TimelapseInvoices.Scripts.Physics.Projectile;

public partial class Projectile : RigidBody2D
{
    // 弹丸速度
    [Export]
    public float Speed = 1000.0f;

    [Export]
    public float MaxDistance = 1000.0f;
    // 经过1000距离销毁
    private Vector2 _startPosition;
    // 伤害
    public float Damage { get; set; } = 10.0f;
    public override void _Ready()
    {
        // 初始化弹丸位置
        _startPosition = GlobalPosition;
        // 设置初始速度
        LinearVelocity = Transform.X * Speed; // 使用Transform.X获取方向向量
    }
    public override void _PhysicsProcess(double delta)
    {
        // 检查弹丸是否超过最大距离
        if (GlobalPosition.DistanceTo(_startPosition) >= MaxDistance)
        {
            QueueFree(); // 超过最大距离后销毁弹丸
        }
    }
    // 碰撞检测
    public void _on_area_2d_body_entered(Node body)
    {
        // 碰撞后销毁弹丸
        QueueFree();
    }

}