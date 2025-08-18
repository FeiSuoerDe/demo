using Godot;
using System;
using System.Collections.Generic;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics.Weapon.WeaponHardpoint;
namespace TimelapseInvoices.Scripts.DataClass;

/// <summary>
/// 船只类型枚举 - 定义了游戏中所有可用的船只类型
/// </summary>
public enum ShipType
{
    /// <summary>航空母舰 - 大型舰艇，可搭载多架飞机</summary>
    AircraftCarrier,

    /// <summary>战列舰 - 重型舰艇，火力强大</summary>
    Battleship,

    /// <summary>巡洋舰 - 中型舰艇，性能均衡</summary>
    Cruiser,

    /// <summary>驱逐舰 - 轻型快速舰艇，机动性好</summary>
    Destroyer,

    /// <summary>护卫舰 - 小型护航舰艇</summary>
    Frigate,

    /// <summary>潜艇 - 水下作战舰艇，隐蔽性强</summary>
    Submarine,

    /// <summary>导弹艇 - 小型高速舰艇，装备导弹</summary>
    MissileBoat,

    /// <summary>炮艇 - 小型浅水舰艇，主要装备舰炮</summary>
    Gunboat
}

/// <summary>
/// 船只数据类 - 包含船只的所有基本属性和状态信息
/// 作为游戏中所有船只的数据模型，支持在Godot编辑器中编辑
/// </summary>
[GlobalClass]
public partial class ShipData : Godot.Resource
{
    #region 基本信息属性
    /// <summary>全局唯一的船只ID - 用于在游戏中唯一标识一艘船</summary>
    [Export]
    public string ShipId { get; set; } = "ship_default_001";

    /// <summary>船只名称 - 显示用的船只名字</summary>
    [Export]
    public string ShipName { get; set; } = "测试驱逐舰";

    /// <summary>船只型号 - 船只的具体型号标识</summary>
    [Export]
    public string ShipModel { get; set; } = "测试型-01";

    /// <summary>描述 - 船只的详细说明文本</summary>
    [Export]
    public string Description { get; set; } = "这是一艘用于测试的标准驱逐舰";

    /// <summary>船只类型 - 决定船只的基本特性和可用装备</summary>
    [Export]
    public ShipType Type { get; set; } = ShipType.Destroyer;

    /// <summary>稀有度 - 影响船只属性上限和获取难度</summary>
    [Export]
    public int Rarity { get; set; } = 1;

    /// <summary>基础价格 - 船只在市场上的基本价值</summary>
    [Export]
    public int BasePrice { get; set; } = 1000;
    #endregion

    #region 性能属性
    /// <summary>加速度 - 船只加速的速率 (单位: 节/秒²)</summary>
    [Export]
    public float Acceleration { get; set; } = 5.0f;

    /// <summary>最大速度 - 船只能达到的最高速度 (单位: 节)</summary>
    [Export]
    public float MaxSpeed { get; set; } = 50.0f;

    /// <summary>当前速度 - 船只当前的速度 (单位: 节)</summary>
    [Export]
    public float CurrentSpeed { get; set; } = 0f;

    /// <summary>转向加速度 - 船只转向的速率 (单位: 度/秒²)</summary>
    [Export]
    public float TurningAcceleration { get; set; } = 200.0f;

    /// <summary>最大转向速度 - 船只能达到的最大转向速率 (单位: 度/秒)</summary>
    [Export]
    public float MaxTurningSpeed { get; set; } = 600.0f;
    #endregion

    #region 战斗属性
    /// <summary>基础护盾值 - 船只的能量护盾强度</summary>
    [Export]
    public float BaseShield { get; set; } = 50f;

    /// <summary>基础装甲值 - 船只的物理装甲强度</summary>
    [Export]
    public float BaseArmor { get; set; } = 30f;

    /// <summary>基础生命值 - 船只的结构完整性</summary>
    [Export]
    public float BaseHealth { get; set; } = 100f;

    /// <summary>辐射耗散速度 - 船只每秒自动降低的辐射值</summary>
    [Export]
    public float RadiationDissipationRate { get; set; } = 1.0f;

    /// <summary>当前辐射值 - 船只当前累积的辐射水平</summary>
    [Export]
    public float CurrentRadiation { get; set; } = 0f;

    /// <summary>最大辐射值 - 船只可承受的最大辐射水平，超过可能导致系统故障</summary>
    [Export]
    public float MaxRadiation { get; set; } = 100f;
    #endregion

    #region 装备属性
    /// <summary>舰船引擎数据列表 - 包含船只所有引擎的数据信息</summary>
    // public List<ShipEngineData> ShipEnginesDatas { get; set; } = new List<ShipEngineData>();

    /// <summary>武器槽位数据列表 - 包含船只所有武器挂载点的数据信息</summary>
    public List<WeaponHardpointData> WeaponHardpointDatas { get; set; } = new List<WeaponHardpointData>();
    #endregion

    #region 构造方法
    /// <summary>
    /// 默认构造方法 - 属性已通过声明时初始化
    /// 适用于在编辑器中创建新的船只数据
    /// </summary>
    public ShipData()
    {
        // 所有基本属性已在声明时初始化，无需再次赋值
    }

    /// <summary>
    /// 简化构造方法 - 仅包含基本参数，其余使用默认值
    /// 适用于快速创建船只原型
    /// </summary>
    /// <param name="shipId">船只ID</param>
    /// <param name="shipName">船只名称</param>
    /// <param name="type">船只类型</param>
    public ShipData(string shipId, string shipName, ShipType type)
    {
        ShipId = shipId;
        ShipName = shipName;
        Type = type;
    }

    /// <summary>
    /// 完整参数构造方法 - 初始化所有船只属性
    /// 适用于程序化生成或导入完整船只数据
    /// </summary>
    /// <param name="shipId">船只ID</param>
    /// <param name="shipName">船只名称</param>
    /// <param name="type">船只类型</param>
    /// <param name="acceleration">加速度</param>
    /// <param name="maxSpeed">最大速度</param>
    /// <param name="turningAcceleration">转向加速度</param>
    /// <param name="maxTurningSpeed">最大转向速度</param>
    /// <param name="basePrice">基础价格</param>
    /// <param name="rarity">稀有度</param>
    /// <param name="baseShield">基础护盾值</param>
    /// <param name="baseArmor">基础装甲值</param>
    /// <param name="baseHealth">基础生命值</param>
    /// <param name="radiationDissipationRate">辐射耗散速度</param>
    /// <param name="currentRadiation">当前辐射值</param>
    /// <param name="maxRadiation">最大辐射值</param>
    public ShipData(string shipId, string shipName, ShipType type, float acceleration, float maxSpeed,
        float turningAcceleration, float maxTurningSpeed, int basePrice, int rarity,
        float baseShield, float baseArmor, float baseHealth,
        float radiationDissipationRate, float currentRadiation, float maxRadiation)
    {
        // 设置基本信息
        ShipId = shipId;
        ShipName = shipName;
        Type = type;
        BasePrice = basePrice;
        Rarity = rarity;

        // 设置性能属性
        Acceleration = acceleration;
        MaxSpeed = maxSpeed;
        CurrentSpeed = 0f; // 初始速度始终为0
        TurningAcceleration = turningAcceleration;
        MaxTurningSpeed = maxTurningSpeed;

        // 设置战斗属性
        BaseShield = baseShield;
        BaseArmor = baseArmor;
        BaseHealth = baseHealth;
        RadiationDissipationRate = radiationDissipationRate;
        CurrentRadiation = currentRadiation;
        MaxRadiation = maxRadiation;
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 克隆当前船只数据 - 创建一个完全独立的数据副本
    /// 用于创建船只实例或进行数据备份
    /// </summary>
    /// <returns>船只数据副本</returns>
    public ShipData Clone()
    {
        var clone = new ShipData(
            ShipId, ShipName, Type, Acceleration, MaxSpeed,
            TurningAcceleration, MaxTurningSpeed, BasePrice, Rarity,
            BaseShield, BaseArmor, BaseHealth,
            RadiationDissipationRate, CurrentRadiation, MaxRadiation
        );
        clone.CurrentSpeed = this.CurrentSpeed;
        return clone;
    }

    /// <summary>
    /// 返回船只信息的字符串表示
    /// 用于日志记录和调试显示
    /// </summary>
    /// <returns>包含船只基本信息的字符串</returns>
    public override string ToString()
    {
        return $"{ShipName} [{Type}] (ID: {ShipId})";
    }

    /// <summary>
    /// 获取船只的完整数据信息
    /// 返回格式化的字符串，包含所有船只属性
    /// </summary>
    /// <returns>格式化的船只完整信息</returns>
    public string GetFullInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        // 基本信息
        sb.AppendLine("=== 基本信息 ===");
        sb.AppendLine($"ID: {ShipId}");
        sb.AppendLine($"名称: {ShipName}");
        sb.AppendLine($"型号: {ShipModel}");
        sb.AppendLine($"类型: {Type}");
        sb.AppendLine($"稀有度: {Rarity}");
        sb.AppendLine($"基础价格: {BasePrice}");
        sb.AppendLine($"描述: {Description}");
        sb.AppendLine();
        
        // 性能属性
        sb.AppendLine("=== 性能属性 ===");
        sb.AppendLine($"加速度: {Acceleration:F2} 节/秒²");
        sb.AppendLine($"最大速度: {MaxSpeed:F2} 节");
        sb.AppendLine($"当前速度: {CurrentSpeed:F2} 节");
        sb.AppendLine($"转向加速度: {TurningAcceleration:F2} 度/秒²");
        sb.AppendLine($"最大转向速度: {MaxTurningSpeed:F2} 度/秒");
        sb.AppendLine();
        
        // 战斗属性
        sb.AppendLine("=== 战斗属性 ===");
        sb.AppendLine($"基础护盾值: {BaseShield:F2}");
        sb.AppendLine($"基础装甲值: {BaseArmor:F2}");
        sb.AppendLine($"基础生命值: {BaseHealth:F2}");
        sb.AppendLine($"辐射耗散速度: {RadiationDissipationRate:F2}/秒");
        sb.AppendLine($"当前辐射值: {CurrentRadiation:F2}/{MaxRadiation:F2}");
        sb.AppendLine();
        
        // 装备属性
        sb.AppendLine("=== 装备信息 ===");
        sb.AppendLine($"武器挂载点数量: {WeaponHardpointDatas.Count}");
        
        // 列出所有武器挂载点
        for (int i = 0; i < WeaponHardpointDatas.Count; i++)
        {
            var hardpoint = WeaponHardpointDatas[i];
            sb.AppendLine($"挂载点 #{i+1}: {(hardpoint != null ? hardpoint.ToString() : "未装配")}");
        }
        
        return sb.ToString();
    }
    #endregion
    // 输出所有数据方法
    
}