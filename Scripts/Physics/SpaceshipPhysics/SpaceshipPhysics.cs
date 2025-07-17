using Godot;
using System;
// 飞船物理体
public partial class SpaceshipPhysics : RigidBody2D
{
    // 
    public float Speed = 200f; // 飞船速度
    public float RotationSpeed = 5f; // 飞船旋转速度
    public override void _Ready()
    {
        // 初始化飞船
        GD.Print("Spaceship is ready.");
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

