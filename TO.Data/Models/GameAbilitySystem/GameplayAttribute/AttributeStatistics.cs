namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性统计信息
/// </summary>
public class AttributeStatistics
{
    /// <summary>
    /// 属性集ID
    /// </summary>
    public Guid AttributeSetId { get; set; }
    
    /// <summary>
    /// 总属性数量
    /// </summary>
    public int TotalAttributes { get; set; }
    
    /// <summary>
    /// 活跃效果数量
    /// </summary>
    public int ActiveEffects { get; set; }
    
    /// <summary>
    /// 总修饰符数量
    /// </summary>
    public int TotalModifiers { get; set; }
    
    /// <summary>
    /// 最小属性值
    /// </summary>
    public float MinAttributeValue { get; set; }
    
    /// <summary>
    /// 最大属性值
    /// </summary>
    public float MaxAttributeValue { get; set; }
    
    /// <summary>
    /// 平均属性值
    /// </summary>
    public float AverageAttributeValue { get; set; }
}