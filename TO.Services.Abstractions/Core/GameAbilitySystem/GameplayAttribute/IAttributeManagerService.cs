using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性管理器服务接口
/// 提供高级的属性管理功能，包括属性集管理、效果应用、批量操作等
/// </summary>
public interface IAttributeManagerService
{
    /// <summary>
    /// 属性变化事件
    /// </summary>
    event Action<Guid, AttributeType, float, float> AttributeChanged;
    
    /// <summary>
    /// 效果应用事件
    /// </summary>
    event Action<Guid, AttributeEffect?> EffectApplied;
    
    /// <summary>
    /// 效果移除事件
    /// </summary>
    event Action<Guid, AttributeEffect> EffectRemoved;
    
    /// <summary>
    /// 注册属性集
    /// </summary>
    /// <param name="attributeSet">属性集</param>
    void RegisterAttributeSet(AttributeSet attributeSet);
    
    /// <summary>
    /// 注销属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    void UnregisterAttributeSet(Guid attributeSetId);
    
    /// <summary>
    /// 获取属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>属性集</returns>
    AttributeSet GetAttributeSet(Guid attributeSetId);
    
    /// <summary>
    /// 获取所有属性集
    /// </summary>
    /// <returns>属性集列表</returns>
    IEnumerable<AttributeSet> GetAllAttributeSets();
    
    /// <summary>
    /// 应用效果到指定属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effect">效果</param>
    /// <returns>是否成功应用</returns>
    bool ApplyEffect(Guid attributeSetId, AttributeEffect? effect);
    
    /// <summary>
    /// 从指定属性集移除效果
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effectId">效果ID</param>
    /// <returns>是否成功移除</returns>
    bool RemoveEffect(Guid attributeSetId, Guid effectId);
    
    /// <summary>
    /// 应用全局效果（影响所有属性集）
    /// </summary>
    /// <param name="effect">全局效果</param>
    void ApplyGlobalEffect(AttributeEffect? effect);
    
    /// <summary>
    /// 移除全局效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    void RemoveGlobalEffect(Guid effectId);
    
    /// <summary>
    /// 批量应用效果
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effects">效果列表</param>
    /// <returns>成功应用的效果数量</returns>
    int ApplyEffects(Guid attributeSetId, IEnumerable<AttributeEffect?> effects);
    
    /// <summary>
    /// 批量移除效果
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effectIds">效果ID列表</param>
    /// <returns>成功移除的效果数量</returns>
    int RemoveEffects(Guid attributeSetId, IEnumerable<Guid> effectIds);
    
    /// <summary>
    /// 更新所有属性集的效果持续时间
    /// </summary>
    /// <param name="deltaTime">时间增量（秒）</param>
    void UpdateEffectDurations(float deltaTime);
    
    /// <summary>
    /// 异步更新效果持续时间
    /// </summary>
    /// <param name="deltaTime">时间增量（秒）</param>
    /// <returns>异步任务</returns>
    Task UpdateEffectDurationsAsync(float deltaTime);
    
    
    /// <summary>
    /// 获取属性值（枚举重载，保持向后兼容）
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="attributeType">属性类型枚举</param>
    /// <returns>属性值，如果不存在则返回null</returns>
    AttributeValue? GetAttributeValue(Guid attributeSetId, AttributeType attributeType);
    
    /// <summary>
    /// 设置属性值（枚举重载，保持向后兼容）
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="attributeType">属性类型枚举</param>
    /// <param name="value">新的属性值</param>
    /// <returns>是否设置成功</returns>
    bool SetAttributeValue(Guid attributeSetId, AttributeType attributeType, float value);
    
    /// <summary>
    /// 查找具有指定效果的属性集
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <returns>具有该效果的属性集列表</returns>
    IEnumerable<AttributeSet> FindAttributeSetsWithEffect(Guid effectId);
    
    /// <summary>
    /// 查找具有指定标签的效果
    /// </summary>
    /// <param name="tag">效果标签</param>
    /// <returns>具有该标签的效果列表</returns>
    IEnumerable<(Guid attributeSetId, AttributeEffect effect)> FindEffectsWithTag(EffectTags tag);
    
    /// <summary>
    /// 获取属性统计信息
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>属性统计信息</returns>
    AttributeStatistics GetAttributeStatistics(Guid attributeSetId);
    
    /// <summary>
    /// 清理过期效果
    /// </summary>
    /// <returns>清理的效果数量</returns>
    int CleanupExpiredEffects();
    
    /// <summary>
    /// 重置属性集到初始状态
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>是否成功重置</returns>
    bool ResetAttributeSet(Guid attributeSetId);
    
    /// <summary>
    /// 获取管理器状态信息
    /// </summary>
    /// <returns>管理器状态</returns>
    ManagerStatus GetStatus();
}