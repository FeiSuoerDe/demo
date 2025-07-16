namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 冲突检查结果
/// </summary>
public class ConflictCheckResult
{
    /// <summary>
    /// 是否有冲突
    /// </summary>
    public bool HasConflicts { get; set; }
    
    /// <summary>
    /// 冲突消息列表
    /// </summary>
    public List<string> ConflictMessages { get; set; } = new List<string>();
    
    /// <summary>
    /// 冲突的修饰器ID列表
    /// </summary>
    public List<Guid> ConflictingModifierIds { get; set; } = new List<Guid>();
}