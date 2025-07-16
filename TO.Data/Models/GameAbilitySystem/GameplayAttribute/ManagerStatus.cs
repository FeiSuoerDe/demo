namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 管理器状态信息
/// </summary>
public class ManagerStatus
{
    /// <summary>
    /// 已注册的属性集数量
    /// </summary>
    public int RegisteredAttributeSets { get; set; }
    
    /// <summary>
    /// 全局效果数量
    /// </summary>
    public int GlobalEffects { get; set; }
    
    /// <summary>
    /// 总活跃效果数量
    /// </summary>
    public int TotalActiveEffects { get; set; }
    
    /// <summary>
    /// 总修饰符数量
    /// </summary>
    public int TotalModifiers { get; set; }
}