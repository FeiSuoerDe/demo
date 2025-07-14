using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

using TO.Repositories.Abstractions.Core.GameAbilitySystem;
using TO.Commons.Enums.Game;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Bases;
using TO.Services.Core.GameAbilitySystem.Base;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;

namespace TO.Services.Core.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 属性管理器服务
    /// 继承自BaseGameAbilityService，减少冗余代码
    /// </summary>
    public class AttributeManagerService : BaseGameAbilityService, IAttributeManagerService
    {
        private readonly IAttributeSetRepo _iAttributeSetRepo;
        private readonly IEffectRepo _iEffectRepo;
        private readonly IAttributeCalculationService _calculationService;

        // 实现接口要求的事件
        public event Action<Guid, AttributeType, float, float> AttributeChanged;
        public event Action<Guid, AttributeEffect?> EffectApplied;
        public event Action<Guid, AttributeEffect> EffectRemoved;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="calculationService">属性计算服务</param>
        /// <param name="iAttributeSetRepo">属性集仓储</param>
        /// <param name="iEffectRepo">效果仓储</param>
        public AttributeManagerService(
            IAttributeCalculationService calculationService,
            IAttributeSetRepo iAttributeSetRepo,
            IEffectRepo iEffectRepo,
            IEventBusRepo eventBusRepo) : base(eventBusRepo)
        {
            _calculationService = calculationService;
            _iAttributeSetRepo = iAttributeSetRepo;
            _iEffectRepo = iEffectRepo;
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
                    var effectsToRemove = attributeSet.GetAppliedEffects().ToList();
                    foreach (var effect in effectsToRemove)
                    {
                        if (effect != null)
                        {
                            attributeSet.RemoveEffect(effect.Id);
                            PublishEvent(new EffectRemoved(attributeSetId.ToString(), effect, attributeSet));
                        }
                    }

                    _iAttributeSetRepo.Remove(attributeSetId);
                }
            });
        }

        /// <summary>
        /// 获取属性集
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>属性集</returns>
        public AttributeSet GetAttributeSet(Guid attributeSetId)
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

                var success = attributeSet.ApplyEffect(effect);
                if (success)
                {
                    EffectApplied?.Invoke(attributeSetId, effect);
                    PublishEvent(new EffectApplied(attributeSetId.ToString(), effect, attributeSet));
                }

                return success;
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

                var effect = attributeSet.GetAppliedEffects().FirstOrDefault(e => e?.Id == effectId);
                var success = attributeSet.RemoveEffect(effectId);

                if (success && effect != null)
                {
                    EffectRemoved?.Invoke(attributeSetId, effect);
                    PublishEvent(new EffectRemoved(attributeSetId.ToString(), effect, attributeSet));
                }

                return success;
            });
        }

        /// <summary>
        /// 应用全局效果（影响所有属性集）
        /// </summary>
        /// <param name="effect">全局效果</param>
        public void ApplyGlobalEffect(AttributeEffect? effect)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            ExecuteWithLock(() =>
            {
                _iEffectRepo.AddGlobalEffect(effect);

                // 应用到所有已注册的属性集
                foreach (var attributeSet in _iAttributeSetRepo.GetAll())
                {
                    if (attributeSet.ApplyEffect(effect))
                    {
                        EffectApplied?.Invoke(attributeSet.Id, effect);
                        PublishEvent(new EffectApplied(attributeSet.Id.ToString(), effect, attributeSet));
                    }
                }
            });
        }

        /// <summary>
        /// 移除全局效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        public void RemoveGlobalEffect(Guid effectId)
        {
            if (effectId == Guid.Empty)
                return;

            ExecuteWithLock(() =>
            {
                var effect = _iEffectRepo.RemoveGlobalEffect(effectId);
                if (effect == null)
                    return;

                // 从所有属性集中移除
                foreach (var attributeSet in _iAttributeSetRepo.GetAll())
                {
                    if (attributeSet.RemoveEffect(effectId))
                    {
                        EffectRemoved?.Invoke(attributeSet.Id, effect);
                        PublishEvent(new EffectRemoved(attributeSet.Id.ToString(), effect, attributeSet));
                    }
                }
            });
        }

        /// <summary>
        /// 批量应用效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effects">效果列表</param>
        /// <returns>成功应用的效果数量</returns>
        public int ApplyEffects(Guid attributeSetId, IEnumerable<AttributeEffect?> effects)
        {
            if (attributeSetId == Guid.Empty || effects == null)
                return 0;

            return ExecuteWithLock(() =>
            {
                var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
                if (attributeSet == null)
                    return 0;

                int successCount = 0;
                foreach (var effect in effects)
                {
                    if (effect != null && attributeSet.ApplyEffect(effect))
                    {
                        EffectApplied?.Invoke(attributeSetId, effect);
                        PublishEvent(new EffectApplied(attributeSetId.ToString(), effect, attributeSet));
                        successCount++;
                    }
                }

                return successCount;
            });
        }

        /// <summary>
        /// 批量移除效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effectIds">效果ID列表</param>
        /// <returns>成功移除的效果数量</returns>
        public int RemoveEffects(Guid attributeSetId, IEnumerable<Guid> effectIds)
        {
            if (attributeSetId == Guid.Empty || effectIds == null)
                return 0;

            return ExecuteWithLock(() =>
            {
                var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
                if (attributeSet == null)
                    return 0;

                int successCount = 0;
                foreach (var effectId in effectIds)
                {
                    if (effectId != Guid.Empty)
                    {
                        var effect = attributeSet.GetAppliedEffects().FirstOrDefault(e => e?.Id == effectId);
                        if (attributeSet.RemoveEffect(effectId))
                        {
                            if (effect != null)
                            {
                                EffectRemoved?.Invoke(attributeSetId, effect);
                                PublishEvent(new EffectRemoved(attributeSetId.ToString(), effect, attributeSet));
                            }
                            successCount++;
                        }
                    }
                }

                return successCount;
            });
        }

        /// <summary>
        /// 获取属性集的所有应用效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>应用的效果列表</returns>
        public IEnumerable<AttributeEffect> GetAppliedEffects(Guid attributeSetId)
        {
            if (attributeSetId == Guid.Empty)
                return Enumerable.Empty<AttributeEffect>();

            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
                return Enumerable.Empty<AttributeEffect>();

            return attributeSet.GetAppliedEffects().Where(e => e != null)!;
        }

        /// <summary>
        /// 清除属性集的所有效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>清除的效果数量</returns>
        public int ClearAllEffects(Guid attributeSetId)
        {
            if (attributeSetId == Guid.Empty)
                return 0;

            return ExecuteWithLock(() =>
            {
                var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
                if (attributeSet == null)
                    return 0;

                var effects = attributeSet.GetAppliedEffects().Where(e => e != null).ToList();
                int clearedCount = 0;

                foreach (var effect in effects)
                {
                    if (effect != null && attributeSet.RemoveEffect(effect.Id))
                    {
                        EffectRemoved?.Invoke(attributeSetId, effect);
                        PublishEvent(new EffectRemoved(attributeSetId.ToString(), effect, attributeSet));
                        clearedCount++;
                    }
                }

                return clearedCount;
            });
        }

        /// <summary>
        /// 检查属性集是否存在指定效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>是否存在该效果</returns>
        public bool HasEffect(Guid attributeSetId, Guid effectId)
        {
            if (attributeSetId == Guid.Empty || effectId == Guid.Empty)
            {
                return false;
            }

            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
            {
                return false;
            }

            return attributeSet.GetAppliedEffects().Any(e => e?.Id == effectId);
        }

        /// <summary>
        /// 获取属性集的统计信息
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>统计信息</returns>
        public AttributeSetStats GetAttributeSetStats(Guid attributeSetId)
        {
            if (attributeSetId == Guid.Empty)
                return new AttributeSetStats();

            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
                return new AttributeSetStats();

            var effects = attributeSet.GetAppliedEffects().Where(e => e != null).ToList();
            
            return new AttributeSetStats
            {
                AttributeSetId = attributeSetId,
                TotalEffects = effects.Count,
                ActiveEffects = effects.Count(e => e.Status == EffectStatus.Active),
                PermanentEffects = effects.Count(e => e.Duration.IsInfinite),
                TemporaryEffects = effects.Count(e => !e.Duration.IsInfinite)
            };
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
                    var effects = attributeSet.GetAppliedEffects().Where(e => e != null).ToList();
                    foreach (var effect in effects)
                    {
                        if (effect != null && !effect.Duration.IsInfinite)
                        {
                            effect.Duration.Update(deltaTime);
                            if (effect.Duration.IsExpired)
                            {
                                attributeSet.RemoveEffect(effect.Id);
                                PublishEvent(new EffectRemoved(attributeSet.Id.ToString(), effect, attributeSet));
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 异步更新效果持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        /// <returns>异步任务</returns>
        public async Task UpdateEffectDurationsAsync(float deltaTime)
        {
            await Task.Run(() => UpdateEffectDurations(deltaTime));
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="attributeType">属性类型</param>
        /// <returns>属性值，如果不存在返回null</returns>
        public AttributeValue? GetAttributeValue(Guid attributeSetId, AttributeType attributeType)
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            return attributeSet?.GetAttribute(attributeType);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="attributeType">属性类型</param>
        /// <param name="value">新值</param>
        /// <returns>是否成功设置</returns>
        public bool SetAttributeValue(Guid attributeSetId, AttributeType attributeType, float value)
        {
            return ExecuteWithLock(() =>
            {
                var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
                if (attributeSet == null) return false;
                
                var oldValue = attributeSet.GetAttribute(attributeType)?.CurrentValue ?? 0f;
                attributeSet.SetAttribute(attributeType, value);
                AttributeChanged?.Invoke(attributeSetId, attributeType, oldValue, value);
                return true;
            });
        }

        /// <summary>
        /// 查找具有指定效果的属性集
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>具有该效果的属性集列表</returns>
        public IEnumerable<AttributeSet> FindAttributeSetsWithEffect(Guid effectId)
        {
            return _iAttributeSetRepo.GetAll()
                .Where(set => set.GetAppliedEffects().Any(e => e?.Id == effectId));
        }

        /// <summary>
        /// 查找具有指定标签的效果
        /// </summary>
        /// <param name="tag">效果标签</param>
        /// <returns>具有该标签的效果列表</returns>
        public IEnumerable<(Guid attributeSetId, AttributeEffect effect)> FindEffectsWithTag(EffectTags tag)
        {
            var results = new List<(Guid, AttributeEffect)>();
            var tagString = tag.ToString();
            foreach (var attributeSet in _iAttributeSetRepo.GetAll())
            {
                var effects = attributeSet.GetAppliedEffects()
                    .Where(e => e != null && e.Tags.Contains(tagString));
                foreach (var effect in effects)
                {
                    if (effect != null)
                        results.Add((attributeSet.Id, effect));
                }
            }
            return results;
        }

        /// <summary>
        /// 获取属性统计信息
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>属性统计信息</returns>
        public AttributeStatistics GetAttributeStatistics(Guid attributeSetId)
        {
            var attributeSet = _iAttributeSetRepo.GetById(attributeSetId);
            if (attributeSet == null)
                return new AttributeStatistics { AttributeSetId = attributeSetId };

            var effects = attributeSet.GetAppliedEffects().Where(e => e != null).ToList();
            var attributes = attributeSet.GetAllAttributeValues().ToList();
            
            return new AttributeStatistics
            {
                AttributeSetId = attributeSetId,
                TotalAttributes = attributes.Count,
                ActiveEffects = effects.Count(e => e.Status == EffectStatus.Active),
                TotalModifiers = effects.Count,
                MinAttributeValue = attributes.Any() ? attributes.Min(a => a.CurrentValue) : 0f,
                MaxAttributeValue = attributes.Any() ? attributes.Max(a => a.CurrentValue) : 0f,
                AverageAttributeValue = attributes.Any() ? attributes.Average(a => a.CurrentValue) : 0f
            };
        }

        /// <summary>
        /// 清理过期效果
        /// </summary>
        /// <returns>清理的效果数量</returns>
        public int CleanupExpiredEffects()
        {
            return ExecuteWithLock(() =>
            {
                int cleanedCount = 0;
                foreach (var attributeSet in _iAttributeSetRepo.GetAll())
                {
                    var expiredEffects = attributeSet.GetAppliedEffects()
                        .Where(e => e != null && !e.Duration.IsInfinite && e.Duration.IsExpired)
                        .ToList();
                    
                    foreach (var effect in expiredEffects)
                    {
                        if (effect != null && attributeSet.RemoveEffect(effect.Id))
                        {
                            PublishEvent(new EffectRemoved(attributeSet.Id.ToString(), effect, attributeSet));
                            cleanedCount++;
                        }
                    }
                }
                return cleanedCount;
            });
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
                var effects = attributeSet.GetAppliedEffects().Where(e => e != null).ToList();
                foreach (var effect in effects)
                {
                    if (effect != null)
                    {
                        attributeSet.RemoveEffect(effect.Id);
                        PublishEvent(new EffectRemoved(attributeSetId.ToString(), effect, attributeSet));
                    }
                }

                // 重新计算所有属性到基础值
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    if (attribute != null)
                    {
                        attribute.SetCurrentValue(attribute.BaseValue);
                    }
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
            var totalEffects = allAttributeSets.SelectMany(set => set.GetAppliedEffects()).Count(e => e != null);
            
            return new ManagerStatus
            {
                RegisteredAttributeSets = allAttributeSets.Count,
                GlobalEffects = _iEffectRepo.GetAllGlobalEffects().Count(),
                TotalActiveEffects = totalEffects,
                TotalModifiers = allAttributeSets.SelectMany(set => set.GetAppliedEffects())
                    .SelectMany(e => e?.Modifiers ?? Enumerable.Empty<AttributeModifier>()).Count()
            };
        }
    }

    /// <summary>
    /// 属性集统计信息
    /// </summary>
    public class AttributeSetStats
    {
        public Guid AttributeSetId { get; set; }
        public int TotalEffects { get; set; }
        public int ActiveEffects { get; set; }
        public int PermanentEffects { get; set; }
        public int TemporaryEffects { get; set; }
    }
}