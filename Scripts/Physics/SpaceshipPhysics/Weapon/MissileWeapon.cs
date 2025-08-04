using Godot;
using System;
// 导弹类
//     /// </summary>

public partial class MissileWeapon : Node2D
{
    [Export]
    public string WeaponName = "导弹武器";

    /// <summary>
    /// 武器的类型
    /// </summary>
    [Export]
    public WeaponData.HardpointType SpecificWeaponType = WeaponData.HardpointType.Missile;

    /// <summary>
    /// 武器的尺寸
    /// </summary>
    [Export]
    public WeaponData.WeaponSize Size = WeaponData.WeaponSize.Medium;

    /// <summary>
    /// 导弹飞行距离
    /// </summary>
    [Export]
    public float Range = 1000.0f;

    /// <summary>
    /// 武器的伤害
    /// </summary>
    [Export]
    public float Damage = 50.0f;

}
