using Godot;
using System;
// 飞船物理体
public partial class SpaceshipPhysics : RigidBody2D
{
    // 
    [Export]
    public float MaxSpeed = 200f; // 飞船速度
    [Export]
    public float Acceleration = 100f; // 飞船加速度
    [Export]
    public float Deceleration = 50f; // 飞船减速度
    [Export]
    public float RotationSpeed = 500f; // 飞船旋转速度
    [Export]
    public float RotationAcceleration = 200f; // 飞船旋转加速度
    [Export]
    // 最大辐能值
    public float MaxEnergy = 100f; // 最大辐能值
                                   // 常态耗散
    [Export]
    public float EnergyDissipation = 0.1f; // 常态耗散
    public override void _Ready()
    {
        // 初始化飞船
        GD.Print("Spaceship is ready.");
    }

    public override void _PhysicsProcess(double delta)
    {
        var force = Vector2.Zero;
        var torque = 0f;

        // W/S 控制前进后退
        if (Input.IsKeyPressed(Key.W)) // W
        {
            force -= Transform.Y * Acceleration;
        }
        if (Input.IsKeyPressed(Key.S)) // S
        {
            force += Transform.Y * Acceleration;
        }

        // Q/E 控制左右旋转
        if (Input.IsKeyPressed(Key.Q))
        {
            torque -= RotationAcceleration * 10;
        }
        if (Input.IsKeyPressed(Key.E))
        {
            torque += RotationAcceleration * 10;
        }

        ApplyCentralForce(force);
        ApplyTorque(torque);

        // 限制最大速度
        if (LinearVelocity.Length() > MaxSpeed)
        {
            LinearVelocity = LinearVelocity.Normalized() * MaxSpeed;
        }
        // 限制最大角速度
        if (Mathf.Abs(AngularVelocity) > RotationSpeed)
        {
            AngularVelocity = Mathf.Sign(AngularVelocity) * RotationSpeed;
        }
    }

    [Export]
    // 引擎槽位节点
    public Node EngineMount; // 引擎槽位节点
    [Export]
    // 武器槽位node
    public Node WeaponHardpoint; // 武器槽位节点
    public bool IsControlled = false; // 是否受控



}
// 引擎类
public partial class Engine : Node2D
{
    // 引擎喷口方向type（上下左右）
    public enum EngineDirection
    {
        Up, // 向上
        Down, // 向下
        Left, // 向左
        Right // 向右
    }
    public EngineDirection Direction; // 引擎喷口方向



}
// 武器槽位
public partial class WeaponHardpoint : Node2D
{
    // 类型分为（能量，导弹，动能）
    public enum WeaponType
    {
        Energy, // 能量武器
        Missile, // 导弹武器
        Kinetic // 动能武器
    }
    public WeaponType Type; // 武器类型
                            // 槽位大小（分为大中小特）
    public enum HardpointSize
    {
        Small, // 小型槽位
        Medium, // 中型槽位
        Large, // 大型槽位
        ExtraLarge // 特大型槽位
    }

}

