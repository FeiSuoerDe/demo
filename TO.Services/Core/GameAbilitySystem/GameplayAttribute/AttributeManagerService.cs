using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Repositories.Abstractions.Core.GameAbilitySystem;
using TO.Commons.Enums.Game;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Core.GameAbilitySystem.Base;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Core.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性管理器服务
/// 继承自BaseGameAbilityService，减少冗余代码
/// </summary>
public class AttributeManagerService : BaseGameAbilityService, IAttributeManagerService
{
    private readonly IAttributeSetRepo _iAttributeSetRepo;
    private readonly IEffectRepo _iEffectRepo;
    private readonly IAbilityEffectService _abilityEffectService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="iAttributeSetRepo">属性集仓储</param>
    /// <param name="iEffectRepo">效果仓储</param>
    /// <param name="eventBusRepo"></param>
    /// <param name="abilityEffectService"></param>
    public AttributeManagerService(
        IAttributeSetRepo iAttributeSetRepo,
        IEffectRepo iEffectRepo,
        IEventBusRepo eventBusRepo,
        IAbilityEffectService abilityEffectService) : base(eventBusRepo)
    {
        _iAttributeSetRepo = iAttributeSetRepo;
        _iEffectRepo = iEffectRepo;
        _abilityEffectService = abilityEffectService;
    }

    /// <summary>
    /// 注册属性集
    /// </summary>
    /// <param name="attributeSet">属性集</param>
    public void RegisterAttributeSet(AttributeSet attributeSet)
    {
        if (attributeSet == null)
            return;

        ExecuteWithLock(() =>
        {
            if (_iAttributeSetRepo.Exists(attributeSet.Id))
            {
                var errorMsg = $"属性集 {attributeSet.Id} 已经注册";
                throw new InvalidOperationException(errorMsg);
            }

            _iAttributeSetRepo.Add(attributeSet);

            // 订阅属性变化事件
            attributeSet.AttributeChanged += (attrType, oldValue, newValue) =>
            {
                PublishEvent(new AttributeChanged(attributeSet.Id, attrType, oldValue, newValue));
            };
            attributeSet.AttributeRangeChanged += (attrType, minValue, maxValue) =>
            {
                PublishEvent(new AttributeRangeChanged(attributeSet.Id, attrType, minValue, maxValue));
            };
        });
    }

    /// <summary>
    /// 注销属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    public void UnregisterAttributeSet(Guid attributeSetId)
    {
        if (attributeSetId == Guid.Empty)
            return;

        ExecuteWithLock(() =>
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet != null)
            {
                // 移除所有应用到该属性集的效果
                _abilityEffectService.ClearAllEffects(attributeSet);

                _iAttributeSetRepo.Remove(attributeSetId);
            }
        });
    }

    /// <summary>
    /// 获取属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>属性集</returns>
    public AttributeSet? GetAttributeSet(Guid attributeSetId)
    {
        if (attributeSetId == Guid.Empty)
            return null;

        return _iAttributeSetRepo.GetById(attributeSetId);
    }

    /// <summary>
    /// 获取所有属性集
    /// </summary>
    /// <returns>属性集列表</returns>
    public IEnumerable<AttributeSet> GetAllAttributeSets()
    {
        return _iAttributeSetRepo.GetAll();
    }

    /// <summary>
    /// 应用效果到指定属性集
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effect">效果</param>
    /// <returns>是否成功应用</returns>
    public bool ApplyEffect(Guid attributeSetId, AttributeEffect? effect)
    {
        if (attributeSetId == Guid.Empty || effect == null)
            return false;

        return ExecuteWithLock(() =>
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
                return false;

            var success = _abilityEffectService.ApplyEffect(effect, attributeSet);
            if (success)
            {
                PublishEvent(new EffectApplied(attributeSetId, effect, attributeSet));
            }

            return false;
        });
    }

    /// <summary>
    /// 从指定属性集移除效果
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="effectId">效果ID</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveEffect(Guid attributeSetId, Guid effectId)
    {
        if (attributeSetId == Guid.Empty || effectId == Guid.Empty)
            return false;

        return ExecuteWithLock(() =>
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
                return false;

            var effect = _abilityEffectService.GetActiveEffects(attributeSet).FirstOrDefault(e => e.Id == effectId);
            var success = _abilityEffectService.RemoveEffect(effectId.ToString(), attributeSet);
            if (success && effect != null)
            {
                PublishEvent(new EffectRemoved(attributeSetId, effect, attributeSet));
            }
            return success;
        });
    }
        

    /// <summary>
    /// 更新所有属性集的效果持续时间
    /// </summary>
    /// <param name="deltaTime">时间增量（秒）</param>
    public void UpdateEffectDurations(float deltaTime)
    {
        ExecuteWithLock(() =>
        {
            foreach (var attributeSet in _iAttributeSetRepo.GetAll())
            {
                _abilityEffectService.UpdateEffects(attributeSet);
                _abilityEffectService.UpdatePeriodicEffects(attributeSet);
            }
        });
    }

    /// <summary>
    /// 获取属性值（枚举重载，保持向后兼容）
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <param name="attributeType">属性类型</param>
    /// <returns>属性值，如果不存在返回null</returns>
    public AttributeValue? GetAttributeValue(Guid attributeSetId, AttributeDefinition attributeType)
    {
        var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
        return attributeSet?.GetAttribute(attributeType);
    }

    /// <summary>
    /// 查找具有指定标签的效果
    /// </summary>
    /// <param name="tag">效果标签</param>
    /// <returns>具有该标签的效果列表</returns>
    public IEnumerable<(Guid attributeSetId, AttributeEffect effect)> FindEffectsWithTag(EffectTags tag)
    {
        var results = new List<(Guid, AttributeEffect)>();
        foreach (var attributeSet in _iAttributeSetRepo.GetAll())
        {
            var effects = _abilityEffectService.GetActiveEffects(attributeSet)
                .Where(e => e.Tags.Contains(tag));
            foreach (var effect in effects)
            {
                results.Add((attributeSet.Id, effect));
            }
        }
        return results;
    }
        

    /// <summary>
    /// 重置属性集到初始状态
    /// </summary>
    /// <param name="attributeSetId">属性集ID</param>
    /// <returns>是否成功重置</returns>
    public bool ResetAttributeSet(Guid attributeSetId)
    {
        return ExecuteWithLock(() =>
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null) return false;

            // 移除所有效果
            _abilityEffectService.ClearAllEffects(attributeSet);

            // 重置所有属性
            foreach (var attr in attributeSet.GetAllAttributeValues())
            {
                attr?.Reset();
            }

            return true;
        });
    }

    /// <summary>
    /// 获取管理器状态信息
    /// </summary>
    /// <returns>管理器状态</returns>
    public ManagerStatus GetStatus()
    {
        var allAttributeSets = _iAttributeSetRepo.GetAll().ToList();
        var totalEffects = allAttributeSets.SelectMany(set => _abilityEffectService.GetActiveEffects(set)).Count(e => e != null);
            
        return new ManagerStatus
        {
            RegisteredAttributeSets = allAttributeSets.Count,
            GlobalEffects = _iEffectRepo.GetAllGlobalEffects().Count(),
            TotalActiveEffects = totalEffects,
            TotalModifiers = allAttributeSets.SelectMany(set => _abilityEffectService.GetActiveEffects(set))
                .SelectMany(e => e?.Modifiers ?? Enumerable.Empty<AttributeModifier>()).Count()
        };
    }
}
