namespace TO.Data.Models.GameAbilitySystem.GameplayEffect;

/// <summary>
/// 修饰器验证结果
/// </summary>
public class ModifierValidationResult
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// 警告消息
    /// </summary>
    public string WarningMessage { get; set; } = string.Empty;
}