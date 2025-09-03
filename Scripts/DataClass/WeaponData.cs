using Godot;

namespace TimelapseInvoices.Scripts.DataClass;

[GlobalClass]
/// <summary>
/// 武器数据资源类，存储武器的所有属性和状态
/// </summary>
public partial class WeaponData : Godot.Resource
{
    #region 武器类型定义

    /// <summary>
    /// 武器槽位类型枚举，定义了武器可以安装在哪种类型的槽位上
    /// </summary>
    public enum HardpointType
    {
        /// <summary>能量武器槽位</summary>
        Energy,
        /// <summary>导弹武器槽位</summary>
        Missile,
        /// <summary>动能武器槽位</summary>
        Kinetic
    }

    /// <summary>
    /// 武器尺寸枚举
    /// </summary>
    public enum WeaponSize
    {
        /// <summary>小型</summary>
        Small,
        /// <summary>中型</summary>
        Medium,
        /// <summary>大型</summary>
        Large,
        /// <summary>特大型</summary>
        ExtraLarge
    }

    /// <summary>
    /// 具体武器类型枚举，定义了游戏中所有可用的武器类型
    /// </summary>


    /// <summary>
    /// 具体武器类型对应的中文名称数组，索引与SpecificWeaponType枚举值对应
    /// </summary>
    public static readonly string[] WeaponTypeNames = {
        "粒子炮",
        "导弹",
        "实弹",
        "光束"
    };

    #endregion

    #region 基本属性

    /// <summary>
    /// 武器的名称
    /// </summary>
    [Export]
    public string WeaponName = "夯";
    [Export]
    // 型号名字
    public string ModelName = "M1";

    /// <summary>
    /// 武器尺寸
    /// </summary>
    [Export]
    public WeaponSize Size = WeaponSize.Small;



    /// <summary>
    /// 当前武器的具体类型枚举
    /// </summary>
    [Export]
    public HardpointType SpecificWeaponType = HardpointType.Energy;
    /// <summary>
    /// 武器射速，表示每秒射击次数
    /// </summary>
    [Export]
    public float FireRate = 1.0f; // 每秒射击次数

    /// <summary>
    /// 武器射程，武器能够打击到的最大距离
    /// </summary>
    [Export]
    public float Range = 1000.0f;

    /// <summary>
    /// 是否为自动武器，false表示朝向鼠标所在方向
    /// </summary>
    [Export]
    public bool IsAutomatic = false; // false：朝向鼠标所在方向

    #endregion

    #region 伤害属性

    /// <summary>
    /// 武器基础伤害值
    /// </summary>
    [Export]
    public int Damage = 10;

    /// <summary>
    /// 对护盾伤害的倍率调整
    /// </summary>
    [Export]
    public float ShieldDamageMultiplier = 0.5f;

    /// <summary>
    /// 对护甲伤害的倍率调整
    /// </summary>
    [Export]
    public float ArmorDamageMultiplier = 1.0f;

    /// <summary>
    /// 对生命值伤害的倍率调整
    /// </summary>
    [Export]
    public float HealthDamageMultiplier = 1.0f;

    #endregion

    #region 弹药相关

    /// <summary>
    /// 武器最大载弹量
    /// </summary>
    [Export]
    public int AmmoCapacity = 100;

    /// <summary>
    /// 武器当前弹药量
    /// </summary>
    [Export]
    public int CurrentAmmo = 100;

    /// <summary>
    /// 武器换弹所需时间，以秒为单位
    /// </summary>
    [Export]
    public float ReloadTime = 2.0f;
    [Export]
    // 武器描述
    public string Description = "一把夯";

    /// <summary>
    /// 武器贴图资源路径
    /// </summary>
    [Export]
    public string TexturePath = "res://Textures/Weapons/DefaultWeapon.png";

    #endregion

    #region 散布相关属性

    /// <summary>
    /// 武器基础散布角度（度），影响射击精准度
    /// </summary>
    [Export(PropertyHint.Range, "0,30,0.1")]
    public float BaseSpreadAngle = 1.0f;

    /// <summary>
    /// 每次射击增加的散布角度（度）
    /// </summary>
    [Export(PropertyHint.Range, "0,10,0.1")]
    public float SpreadIncreasePerShot = 0.5f;

    /// <summary>
    /// 最大散布角度（度）
    /// </summary>
    [Export(PropertyHint.Range, "0,45,0.5")]
    public float MaxSpreadAngle = 15.0f;

    /// <summary>
    /// 散布恢复速率（度/秒）
    /// </summary>
    [Export(PropertyHint.Range, "0,20,0.1")]
    public float SpreadRecoveryRate = 2.0f;

    #endregion
}