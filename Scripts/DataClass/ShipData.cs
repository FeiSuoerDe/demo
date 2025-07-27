using Godot;
using System;
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
public partial class ShipData : Resource
{
    #region 默认值设置
    /// <summary>默认船只类型 - 新建船只时使用的默认类型</summary>
    [Export]
    public ShipType DEFAULT_SHIP_TYPE = ShipType.Destroyer;

    /// <summary>默认生命值 - 新建船只的基础生命值</summary>
    [Export]
    public float DEFAULT_HEALTH = 100f;

    /// <summary>默认稀有度 - 新建船只的基础稀有度等级</summary>
    [Export]
    public int DEFAULT_RARITY = 1;

    /// <summary>默认辐射耗散速度 - 船只每秒自动减少的辐射值</summary>
    [Export]
    public float DEFAULT_RADIATION_DISSIPATION_RATE = 1.0f;

    /// <summary>默认当前辐射值 - 新建船只的初始辐射水平</summary>
    [Export]
    public float DEFAULT_CURRENT_RADIATION = 0f;

    /// <summary>默认最大辐射值 - 船只可承受的最大辐射水平</summary>
    [Export]
    public float DEFAULT_MAX_RADIATION = 100f;

    /// <summary>默认当前速度 - 新建船只的初始速度</summary>
    [Export]
    public float DEFAULT_CURRENT_SPEED = 0f;
    #endregion

    #region 基本信息属性
    /// <summary>全局唯一的船只ID - 用于在游戏中唯一标识一艘船</summary>
    [Export]
    public string ShipId { get; set; }

    /// <summary>船只名称 - 显示用的船只名字</summary>
    [Export]
    public string ShipName { get; set; }

    /// <summary>船只类型 - 决定船只的基本特性和可用装备</summary>
    [Export]
    public ShipType Type { get; set; }

    /// <summary>稀有度 - 影响船只属性上限和获取难度</summary>
    [Export]
    public int Rarity { get; set; }

    /// <summary>基础价格 - 船只在市场上的基本价值</summary>
    [Export]
    public int BasePrice { get; set; }
    #endregion

    #region 性能属性
    /// <summary>加速度 - 船只加速的速率 (单位: 节/秒²)</summary>
    [Export]
    public float Acceleration { get; set; }

    /// <summary>最大速度 - 船只能达到的最高速度 (单位: 节)</summary>
    [Export]
    public float MaxSpeed { get; set; }

    /// <summary>当前速度 - 船只当前的速度 (单位: 节)</summary>
    [Export]
    public float CurrentSpeed { get; set; }

    /// <summary>转向加速度 - 船只转向的速率 (单位: 度/秒²)</summary>
    [Export]
    public float TurningAcceleration { get; set; }

    /// <summary>最大转向速度 - 船只能达到的最大转向速率 (单位: 度/秒)</summary>
    [Export]
    public float MaxTurningSpeed { get; set; }
    #endregion

    #region 战斗属性
    /// <summary>基础护盾值 - 船只的能量护盾强度</summary>
    [Export]
    public float BaseShield { get; set; }

    /// <summary>基础装甲值 - 船只的物理装甲强度</summary>
    [Export]
    public float BaseArmor { get; set; }

    /// <summary>基础生命值 - 船只的结构完整性</summary>
    [Export]
    public float BaseHealth { get; set; }

    /// <summary>辐射耗散速度 - 船只每秒自动降低的辐射值</summary>
    [Export]
    public float RadiationDissipationRate { get; set; }

    /// <summary>当前辐射值 - 船只当前累积的辐射水平</summary>
    [Export]
    public float CurrentRadiation { get; set; }

    /// <summary>最大辐射值 - 船只可承受的最大辐射水平，超过可能导致系统故障</summary>
    [Export]
    public float MaxRadiation { get; set; }
    #endregion

    #region 构造方法
    /// <summary>
    /// 默认构造方法 - 使用默认值初始化所有属性
    /// 适用于在编辑器中创建新的船只数据
    /// </summary>
    public ShipData()
    {
        // 初始化基本信息
        ShipId = string.Empty;
        ShipName = string.Empty;
        Type = DEFAULT_SHIP_TYPE;
        BasePrice = 0;
        Rarity = DEFAULT_RARITY;

        // 初始化性能属性
        Acceleration = 0f;
        MaxSpeed = 0f;
        CurrentSpeed = DEFAULT_CURRENT_SPEED;
        TurningAcceleration = 0f;
        MaxTurningSpeed = 0f;

        // 初始化战斗属性
        BaseShield = 0f;
        BaseArmor = 0f;
        BaseHealth = DEFAULT_HEALTH;
        RadiationDissipationRate = DEFAULT_RADIATION_DISSIPATION_RATE;
        CurrentRadiation = DEFAULT_CURRENT_RADIATION;
        MaxRadiation = DEFAULT_MAX_RADIATION;
    }

    /// <summary>
    /// 简化构造方法 - 仅包含基本参数，其余使用默认值
    /// 适用于快速创建船只原型
    /// </summary>
    /// <param name="shipId">船只ID</param>
    /// <param name="shipName">船只名称</param>
    /// <param name="type">船只类型</param>
    public ShipData(string shipId, string shipName, ShipType type) : this()
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
        CurrentSpeed = 0f; // 初始速度为0
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
    #endregion
}