using Godot;

// ShipResource.cs
namespace TimelapseInvoices.Scripts.Resource;

/// <summary>
/// 飞船资源定义，使用Godot的Resource系统存储飞船属性
/// 可以创建.tres资源文件来配置不同飞船
/// /// </summary>
[GlobalClass]
public partial class ShipResource : Godot.Resource
{
    [Export] public string ShipName { get; set; } // 飞船名称
    [Export] public Texture2D Icon { get; set; }   // 飞船图标，用于UI显示
    [Export] public float MaxSpeed { get; set; }  // 最大速度
    [Export] public float Acceleration { get; set; } // 加速度
    [Export] public float TurnSpeed { get; set; } // 转向速度
    [Export] public float ShieldCapacity { get; set; } // 盾牌容量
    [Export] public float Armor { get; set; } // 护甲值
    [Export] public PackedScene WeaponScene { get; set; } // 武器场景
}