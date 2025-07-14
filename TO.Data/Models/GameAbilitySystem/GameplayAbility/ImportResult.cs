namespace TO.Data.Models.GameAbilitySystem.GameplayAbility;

/// <summary>
/// 技能导入结果
/// </summary>
public class AbilityImportResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int ImportedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    
    public int TotalProcessed => ImportedCount + UpdatedCount + SkippedCount;
}