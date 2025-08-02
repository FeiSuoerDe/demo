using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Repositories.Abstractions.Core.GameAbilitySystem;

/// <summary>
/// 效果仓储接口
/// </summary>
public interface IEffectRepo
{
    /// <summary>
    /// 添加全局效果
    /// </summary>
    /// <param name="effect">效果</param>
    void AddGlobalEffect(AttributeEffect effect);
    
    /// <summary>
    /// 移除全局效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <returns>被移除的效果，如果不存在返回null</returns>
    AttributeEffect? RemoveGlobalEffect(Guid effectId);
    
    /// <summary>
    /// 获取所有全局效果
    /// </summary>
    /// <returns>全局效果列表</returns>
    IEnumerable<AttributeEffect> GetAllGlobalEffects();
    
    /// <summary>
    /// 查找具有指定标签的全局效果
    /// </summary>
    /// <param name="tag">效果标签</param>
    /// <returns>具有该标签的效果列表</returns>
    IEnumerable<AttributeEffect> FindGlobalEffectsWithTag(EffectTags tag);
    
    /// <summary>
    /// 获取已过期的全局效果
    /// </summary>
    /// <returns>已过期的全局效果列表</returns>
    IEnumerable<AttributeEffect> GetExpiredGlobalEffects();
    
    /// <summary>
    /// 移除已过期的全局效果
    /// </summary>
    /// <returns>被移除的过期效果列表</returns>
    IEnumerable<AttributeEffect> RemoveExpiredGlobalEffects();
    
    /// <summary>
    /// 清理过期的全局效果
    /// </summary>
    /// <returns>清理的效果数量</returns>
    int CleanupExpiredGlobalEffects();
    
    /// <summary>
    /// 清空所有全局效果
    /// </summary>
    void ClearGlobalEffects();
    
    /// <summary>
    /// 全局效果数量
    /// </summary>
    int GlobalEffectCount { get; }
}