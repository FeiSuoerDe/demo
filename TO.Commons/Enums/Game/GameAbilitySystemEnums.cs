namespace TO.Commons.Enums.Game
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        // 通用属性
        Health,
        Energy,
        Speed,
        
        // 角色核心属性（基于需求文档）
        Intelligence,    // 智力
        Perception,      // 感知
        Charisma,        // 魅力
        Will,            // 意志
        Constitution,    // 体质
        Agility,         // 敏捷
        
        // 角色派生属性
        LifeValue,       // 生命值（基于体质计算）
        MentalValue,     // 精神值（基于意志计算）
        MovementSpeed,   // 移动速度（基于敏捷计算）
        
        // 飞船核心属性（基于需求文档）
        Thrust,          // 推进力
        Shield,          // 护盾
        Armor,           // 装甲
        Maneuverability, // 机动性
        Sensors,         // 传感器
        
        // 飞船规格属性
        Mass,            // 质量
        Length,          // 长度
        Width,           // 宽度
        Height,          // 高度
        CargoCapacity,   // 货舱容量
        FuelCapacity,    // 燃料容量
        
        // 扩展属性（预留）
        CustomAttribute1,
        CustomAttribute2,
        CustomAttribute3
    }

    /// <summary>
    /// 修饰器来源类型
    /// </summary>
    public enum SourceType
    {
        Equipment,      // 装备
        Skill,          // 技能
        Buff,           // 增益效果
        Environment,    // 环境
        System          // 系统
    }

    /// <summary>
    /// 修饰器操作类型
    /// </summary>
    public enum ModifierOperationType
    {
        Add,           // 加法修饰
        Multiply,      // 乘法修饰
        Override,      // 覆盖修饰
        Percentage     // 百分比修饰
    }

    /// <summary>
    /// 效果类型
    /// </summary>
    public enum EffectType
    {
        Buff,       // 正面效果
        Debuff,     // 负面效果
        Neutral     // 中性效果
    }

    /// <summary>
    /// 效果标签（可组合）
    /// </summary>
    [Flags]
    public enum EffectTags
    {
        None = 0,
        Physical = 1 << 0,
        Mental = 1 << 1,
        Environmental = 1 << 2,
        Magical = 1 << 3,
        Technological = 1 << 4,
        Temporary = 1 << 5,
        Permanent = 1 << 6
    }

    /// <summary>
    /// 效果堆叠类型
    /// </summary>
    public enum EffectStackingType
    {
        NoStack,    // 不堆叠，应用新效果时替换旧效果
        Stack,      // 堆叠，增加层数
        Replace,    // 替换，应用新效果时完全替换旧效果
        Duration    // 仅刷新持续时间
    }
}