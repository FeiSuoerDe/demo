using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性计算服务接口
/// 定义属性修饰器计算和验证的核心功能
/// </summary>
public interface IAttributeCalculationService
{
    /// <summary>
    /// 计算属性的最终值
    /// </summary>
    /// <param name="baseValue">基础值</param>
    /// <param name="modifiers">修饰器列表</param>
    /// <returns>计算后的最终值</returns>
    float CalculateAttributeValue(float baseValue, IEnumerable<AttributeModifier> modifiers);
    
    /// <summary>
    /// 计算修饰器对属性值的贡献
    /// </summary>
    /// <param name="baseValue">基础值</param>
    /// <param name="modifier">修饰器</param>
    /// <param name="allModifiers">所有修饰器（用于上下文计算）</param>
    /// <returns>修饰器的贡献值</returns>
    float CalculateModifierContribution(float baseValue, AttributeModifier modifier, 
                                        IEnumerable<AttributeModifier> allModifiers);
    
    /// <summary>
    /// 验证修饰器是否有效
    /// </summary>
    /// <param name="modifier">修饰器</param>
    /// <returns>验证结果</returns>
    ModifierValidationResult ValidateModifier(AttributeModifier modifier);
    
    /// <summary>
    /// 批量验证修饰器
    /// </summary>
    /// <param name="modifiers">修饰器列表</param>
    /// <returns>验证结果列表</returns>
    IEnumerable<ModifierValidationResult> ValidateModifiers(IEnumerable<AttributeModifier> modifiers);
    
    /// <summary>
    /// 检查修饰器冲突
    /// </summary>
    /// <param name="modifiers">修饰器列表</param>
    /// <returns>冲突检查结果</returns>
    ConflictCheckResult CheckModifierConflicts(IEnumerable<AttributeModifier> modifiers);
    
    /// <summary>
    /// 优化修饰器列表（移除冗余、合并同类型等）
    /// </summary>
    /// <param name="modifiers">修饰器列表</param>
    /// <returns>优化后的修饰器列表</returns>
    IEnumerable<AttributeModifier> OptimizeModifiers(IEnumerable<AttributeModifier> modifiers);
}