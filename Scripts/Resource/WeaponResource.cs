using Godot;

// WeaponResource.cs
namespace TimelapseInvoices.Scripts.Resource;

/// <summary>
/// 武器资源定义，使用Godot的Resource系统存储武器属性
/// 可以创建.tres资源文件来配置不同武器
/// </summary>
[GlobalClass]
public partial class WeaponResource : Godot.Resource
{
    [Export] public string WeaponName { get; set; } // 武器名称
    [Export] public Texture2D Icon { get; set; }   // 武器图标，用于UI显示
    [Export] public float Damage { get; set; }     // 武器基础伤害值
    [Export] public float FireRate { get; set; }   // 每秒发射次数
    [Export] public float Range { get; set; }      // 武器有效射程
    [Export] public PackedScene ProjectileScene { get; set; } // 弹丸场景
    [Export] public float EnergyCost { get; set; } // 每次射击消耗的能量
    [Export] public AudioStream ShootSound { get; set; } // 射击音效
}