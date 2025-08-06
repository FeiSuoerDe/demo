
using Godot;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayEffect;
using TO.Services.Core.GameAbilitySystem.Base;


namespace TO.Services.Core.GameAbilitySystem.GameplayEffect;

/// <summary>
/// 重构后的技能效果管理服务
/// 使用基础类和帮助类来减少冗余代码
/// </summary>
public class AbilityEffectService : BaseGameAbilityService, IAbilityEffectService
{
    
    private readonly Dictionary<Guid, List<ActiveEffect>> _activeEffects;
    private readonly Dictionary<Guid, Dictionary<string, EffectStack>> _stackableEffects;
    private readonly Dictionary<Guid, DateTime> _nextExpiryTimes;
    private readonly Dictionary<Guid, List<PeriodicEffect>> _periodicEffects;
    private readonly Dictionary<Guid, DateTime> _nextPeriodicTimes;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventBusRepo">事件总线仓储</param>
    public AbilityEffectService(IEventBusRepo eventBusRepo) : base(eventBusRepo)
    {
        _activeEffects = new Dictionary<Guid, List<ActiveEffect>>();
        _stackableEffects = new Dictionary<Guid, Dictionary<string, EffectStack>>();
        _nextExpiryTimes = new Dictionary<Guid, DateTime>();
        _periodicEffects = new Dictionary<Guid, List<PeriodicEffect>>();
        _nextPeriodicTimes = new Dictionary<Guid, DateTime>();
    }
        
    /// <summary>
    /// 应用效果到目标
    /// </summary>
    /// <param name="effect">游戏效果</param>
    /// <param name="target">目标属性集</param>
    /// <param name="source">源属性集</param>
    /// <param name="sourceAbilityId">源技能ID</param>
    /// <returns>是否成功应用</returns>
    public bool ApplyEffect(AttributeEffect effect, AttributeSet target, 
        AttributeSet? source = null, string? sourceAbilityId = null)
    {
        if (effect == null || target == null)
            return false;
            
        return ExecuteWithLock(() =>
        {
            // 检查是否可以应用
            if (!CanApplyEffect(effect, target, source))
                return false;
                
            try
            {
                // 新数据流设计：基于 EffectType + IsPeriodic 组合
                // 1. Instant + Non-Periodic: 立即执行
                // 2. Duration + Non-Periodic: 持续效果（有过期时间）
                // 3. Duration + Periodic: 周期持续效果（有过期时间 + 周期执行）
                // 4. Infinite + Non-Periodic: 无限效果（无过期时间）
                // 5. Infinite + Periodic: 无限周期效果（无过期时间 + 周期执行）

                return effect.EffectType switch
                {
                    EffectType.Instant => ProcessInstantEffect(effect, target, source, sourceAbilityId),
                    EffectType.Duration => effect.IsPeriodic 
                        ? ProcessDurationPeriodicEffect(effect, target, source, sourceAbilityId)
                        : ProcessDurationEffect(effect, target, source, sourceAbilityId),
                    EffectType.Infinite => effect.IsPeriodic 
                        ? ProcessInfinitePeriodicEffect(effect, target, source, sourceAbilityId)
                        : ProcessInfiniteEffect(effect, target, source, sourceAbilityId),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[AbilityEffectService] 应用效果时发生错误: {ex.Message}");
                return false;
            }
        });
    }


    /// <summary>
    /// 移除效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <param name="target">目标属性集</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveEffect(string effectIdStr, AttributeSet target)
    {
        if (!Guid.TryParse(effectIdStr, out Guid effectId))
            return false;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_activeEffects.ContainsKey(targetId))
                return false;
                
            var effects = _activeEffects[targetId];
            var effectToRemove = effects.FirstOrDefault(e => e.Effect.Id == effectId);
                
            if (effectToRemove == null)
                return false;
                
            try
            {
                // 移除效果的影响
                UnapplyEffectModifications(effectToRemove.Effect, target);
                    
                effects.Remove(effectToRemove);
                    
                if (effects.Count == 0) {
                    _activeEffects.Remove(targetId);
                    _nextExpiryTimes.Remove(targetId);
                } else {
                    _nextExpiryTimes[targetId] = effects.Min(e => e.ExpiryTime);
                }
                    
                // 处理可叠加效果
                if (effectToRemove.Effect.StackingType != EffectStackingType.NoStack)
                {
                    var effectKey = $"{effectToRemove.Effect.Name}_{effectToRemove.Effect.EffectType}";
                    if (_stackableEffects.TryGetValue(targetId, out var innerDict) && innerDict.TryGetValue(effectKey, out var stack))
                    {
                        stack.CurrentStacks = Math.Max(0, stack.CurrentStacks - 1);
                        if (stack.CurrentStacks == 0)
                            innerDict.Remove(effectKey);
                        if (innerDict.Count == 0)
                            _stackableEffects.Remove(targetId);
                    }
                }
                    
                PublishEvent(new EffectRemoved(targetId, effectToRemove.Effect, target));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        });
    }
    
        
    /// <summary>
    /// 清除目标的所有效果
    /// </summary>
    /// <param name="target">目标属性集</param>
    /// <returns>清除的效果数量</returns>
    public int ClearAllEffects(AttributeSet target)
    {
        if (target == null)
            return 0;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_activeEffects.ContainsKey(targetId))
                return 0;
                
            var effects = _activeEffects[targetId];
            var removedCount = effects.Count;
                
            try
            {
                foreach (var effect in effects.ToList())
                {
                    UnapplyEffectModifications(effect.Effect, target);
                    PublishEvent(new EffectRemoved(targetId, effect.Effect, target));
                }
                    
                _activeEffects.Remove(targetId);
                _stackableEffects.Remove(targetId);
                _nextExpiryTimes.Remove(targetId);
                    
                return removedCount;
            }
            catch (Exception)
            {
                return 0;
            }
        });
    }

    /// <summary>
    /// 移除目标的所有周期效果
    /// </summary>
    /// <param name="target">目标属性集</param>
    /// <returns>移除的周期效果数量</returns>
    public int RemoveAllPeriodicEffects(AttributeSet target)
    {
        if (target == null)
            return 0;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_periodicEffects.ContainsKey(targetId))
                return 0;
                
            var effects = _periodicEffects[targetId];
            var removedCount = effects.Count;
                
            try
            {
                foreach (var periodicEffect in effects.ToList())
                {
                    PublishEvent(new EffectRemoved(targetId, periodicEffect.Effect, target));
                    GD.Print($"[AbilityEffectService] 周期效果已移除: EffectId={periodicEffect.Effect.Id}, Target={targetId}");
                }
                    
                _periodicEffects.Remove(targetId);
                _nextPeriodicTimes.Remove(targetId);
                    
                return removedCount;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[AbilityEffectService] 移除周期效果错误: {ex.Message}");
                return 0;
            }
        });
    }

    /// <summary>
    /// 移除指定的周期效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <param name="target">目标属性集</param>
    /// <returns>是否成功移除</returns>
    public bool RemovePeriodicEffect(string effectId, AttributeSet target)
    {
        if (string.IsNullOrEmpty(effectId) || target == null)
            return false;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_periodicEffects.ContainsKey(targetId))
                return false;
                
            var effects = _periodicEffects[targetId];
            var effectToRemove = effects.FirstOrDefault(e => e.Effect.Id.ToString() == effectId);
                
            if (effectToRemove == null)
                return false;
                
            try
            {
                effects.Remove(effectToRemove);
                PublishEvent(new EffectRemoved(targetId, effectToRemove.Effect, target));
                GD.Print($"[AbilityEffectService] 周期效果已移除: EffectId={effectId}, Target={targetId}");
                
                // 更新下次执行时间
                if (effects.Count > 0)
                {
                    var nextTime = effects.Min(e => e.NextExecutionTime);
                    _nextPeriodicTimes[targetId] = nextTime;
                }
                else
                {
                    _periodicEffects.Remove(targetId);
                    _nextPeriodicTimes.Remove(targetId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[AbilityEffectService] 移除周期效果错误: {ex.Message}");
                return false;
            }
        });
    }
        
    /// <summary>
    /// 检查目标是否有指定效果
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <param name="target">目标属性集</param>
    /// <returns>是否有该效果</returns>
    public bool HasEffect(string effectId, AttributeSet target)
    {
        if (string.IsNullOrEmpty(effectId) || target == null)
            return false;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            return _activeEffects.ContainsKey(targetId) && 
                   _activeEffects[targetId].Any(e => e.Effect.Id.ToString() == effectId);
        });
    }
        
    /// <summary>
    /// 获取目标的所有活跃效果
    /// </summary>
    /// <param name="target">目标属性集</param>
    /// <returns>活跃效果列表</returns>
    public IEnumerable<AttributeEffect> GetActiveEffects(AttributeSet target)
    {
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_activeEffects.ContainsKey(targetId))
                return new List<AttributeEffect>();
                
            return _activeEffects[targetId].Select(ae => ae.Effect).ToList();
        });
    }
    
    /// <summary>
    /// 更新效果（移除过期效果）
    /// </summary>
    /// <param name="target">目标属性集</param>
    /// <returns>移除的过期效果数量</returns>
    public int UpdateEffects(AttributeSet target)
    {
        if (target == null)
            return 0;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_activeEffects.ContainsKey(targetId))
                return 0;
                
            var effects = _activeEffects[targetId];
            var now = DateTime.UtcNow;
            if (_nextExpiryTimes.TryGetValue(targetId, out var nextExpiry) && now < nextExpiry) return 0;
            var expiredEffects = effects.Where(e => e.ExpiryTime <= now).ToList();
            var removedCount = 0;
            var innerDict = _stackableEffects.GetValueOrDefault(targetId);
                
            try
            {
                foreach (var expiredEffect in expiredEffects)
                {
                    UnapplyEffectModifications(expiredEffect.Effect, target);
                    effects.Remove(expiredEffect);
                        
                    // 处理可叠加效果
                    if (expiredEffect.Effect.StackingType != EffectStackingType.NoStack)
                    {
                        if (innerDict != null)
                        {
                            var effectKey = $"{expiredEffect.Effect.Name}_{expiredEffect.Effect.EffectType}";
                            if (innerDict.TryGetValue(effectKey, out var stack))
                            {
                                stack.CurrentStacks = Math.Max(0, stack.CurrentStacks - 1);
                                 
                                if (stack.CurrentStacks == 0)
                                    innerDict.Remove(effectKey);
                            }
                        }
                    }
                        
                    PublishEvent(new EffectExpired(targetId, expiredEffect.Effect, target));
                    removedCount++;
                }
                    
                if (effects.Count == 0)
                {
                    _activeEffects.Remove(targetId);
                    _nextExpiryTimes.Remove(targetId);
                }
                else
                {
                    // 更新下次过期时间
                    var nextExpiryTime = effects.Min(e => e.ExpiryTime);
                    _nextExpiryTimes[targetId] = nextExpiryTime;
                }
                    
                if (innerDict != null && innerDict.Count == 0)
                    _stackableEffects.Remove(targetId);
                    
                return removedCount;
            }
            catch (Exception)
            {
                return removedCount;
            }
        });
    }

    /// <summary>
    /// 更新周期效果（执行到期的周期效果）
    /// </summary>
    /// <param name="target">目标属性集</param>
    /// <returns>执行的周期效果数量</returns>
    public int UpdatePeriodicEffects(AttributeSet target)
    {
        if (target == null)
            return 0;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_periodicEffects.ContainsKey(targetId))
                return 0;
                
            var effects = _periodicEffects[targetId];
            var now = DateTime.UtcNow;
            if (_nextPeriodicTimes.TryGetValue(targetId, out var nextExecution) && now < nextExecution) 
                return 0;
                
            // 移除过期的周期效果
            var expiredEffects = effects.Where(e => e.ExpiryTime <= now).ToList();
            foreach (var expiredEffect in expiredEffects)
            {
                effects.Remove(expiredEffect);
                GD.Print($"[AbilityEffectService] 周期效果过期移除: EffectId={expiredEffect.Effect.Id}, Target={targetId}");
            }
            
            var readyEffects = effects.Where(e => e.NextExecutionTime <= now && e.ExpiryTime > now).ToList();
            var executedCount = 0;
                
            try
            {
                foreach (var periodicEffect in readyEffects)
                {
                    // 执行周期效果
                    ApplyEffectModifications(periodicEffect.Effect, target);
                    periodicEffect.ExecutionCount++;
                    
                    // 设置下次执行时间
                    periodicEffect.NextExecutionTime = now.AddSeconds(periodicEffect.IntervalSeconds);
                    
                    PublishEvent(new EffectApplied(targetId, periodicEffect.Effect, target));
                    GD.Print($"[AbilityEffectService] 周期效果执行: EffectId={periodicEffect.Effect.Id}, Target={targetId}, ExecutionCount={periodicEffect.ExecutionCount}");
                    
                    executedCount++;
                }
                
                // 更新下次执行时间
                if (effects.Count > 0)
                {
                    var nextTime = effects.Min(e => e.NextExecutionTime);
                    _nextPeriodicTimes[targetId] = nextTime;
                }
                else
                {
                    _nextPeriodicTimes.Remove(targetId);
                }
                
                return executedCount;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[AbilityEffectService] 周期效果更新错误: {ex.Message}");
                return executedCount;
            }
        });
    }
    
        
    #region 私有方法
        
    //TODO: 检查互斥效果,检查前置条件
    private bool CanApplyEffect(AttributeEffect effect, AttributeSet target, AttributeSet? source)
    {
            
        return true;
    }
        
    // 旧的Apply方法已被新的Process方法和辅助方法替代，以实现更清晰的数据流
    
    /// <summary>
    /// 根据效果类型获取过期时间
    /// </summary>
    private DateTime GetExpiryTimeByEffectType(AttributeEffect effect)
    {
        return effect.EffectType switch
        {
            EffectType.Duration => DateTime.UtcNow.AddSeconds(effect.Duration.TotalTime),
            EffectType.Infinite => DateTime.MaxValue,
            _ => DateTime.UtcNow.AddSeconds(effect.Duration.TotalTime)
        };
    }

    /// <summary>
    /// 处理即时效果：立即应用修改 -> 发布事件 -> 完成
    /// </summary>
    private bool ProcessInstantEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 即时效果：立即应用修改，不存储到活动效果中
        ApplyEffectModifications(effect, target);
        
        var targetId = target.Id;
        PublishEvent(new EffectApplied(targetId, effect, target));
        
        GD.Print($"[AbilityEffectService] 即时效果已应用: EffectId={effect.Id}, Target={targetId}");
        return true;
    }

    /// <summary>
    /// 处理持续效果：应用修改 -> 存储到ActiveEffects -> 设置过期时间 -> 发布事件
    /// </summary>
    private bool ProcessDurationEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 应用修改
        ApplyEffectModifications(effect, target);
        
        // 存储效果
        if (effect.StackingType == EffectStackingType.NoStack)
        {
            StoreActiveEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreStackableEffect(effect, target, source, sourceAbilityId);
        }
        
        // 发布事件
        PublishEvent(new EffectApplied(target.Id, effect, target));
        return true;
    }

    /// <summary>
    /// 存储普通活跃效果
    /// </summary>
    private void StoreActiveEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        var targetId = target.Id;
        var activeEffect = new ActiveEffect
        {
            Effect = effect,
            AppliedTime = DateTime.UtcNow,
            ExpiryTime = GetExpiryTimeByEffectType(effect),
            Source = source,
            SourceAbilityId = sourceAbilityId
        };
        
        if (!_activeEffects.ContainsKey(targetId))
            _activeEffects[targetId] = new List<ActiveEffect>();
            
        _activeEffects[targetId].Add(activeEffect);
        
        if (effect.EffectType == EffectType.Duration)
        {
            UpdateExpiryTime(targetId, activeEffect.ExpiryTime);
        }
    }
    
    /// <summary>
    /// 存储可堆叠活跃效果
    /// </summary>
    private void StoreStackableEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        var targetId = target.Id;
        var effectKey = $"{effect.Name}_{effect.EffectType}";
        
        if (!_stackableEffects.ContainsKey(targetId))
            _stackableEffects[targetId] = new Dictionary<string, EffectStack>();
            
        if (_stackableEffects[targetId].TryGetValue(effectKey, out var stack))
        {
            stack.CurrentStacks = Math.Min(stack.CurrentStacks + 1, effect.MaxStacks);
        }
        else
        {
            _stackableEffects[targetId][effectKey] = new EffectStack
            {
                EffectKey = effectKey,
                MaxStacks = effect.MaxStacks,
                CurrentStacks = 1
            };
        }
        
        StoreActiveEffect(effect, target, source, sourceAbilityId);
    }
    
    /// <summary>
    /// 存储周期效果
    /// </summary>
    private void StorePeriodicEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        var targetId = target.Id;
        var intervalSeconds = effect.IntervalSeconds > 0 ? effect.IntervalSeconds : 1.0;
        
        // 根据效果类型设置过期时间
        var expiryTime = effect.EffectType == EffectType.Duration 
            ? DateTime.UtcNow.AddSeconds(effect.Duration.TotalTime)
            : DateTime.MaxValue;
        
        var periodicEffect = new PeriodicEffect
        {
            Effect = effect,
            AppliedTime = DateTime.UtcNow,
            ExpiryTime = expiryTime,
            NextExecutionTime = DateTime.UtcNow.AddSeconds(intervalSeconds),
            IntervalSeconds = intervalSeconds,
            Source = source,
            SourceAbilityId = sourceAbilityId,
            ExecutionCount = 0
        };
        
        if (!_periodicEffects.ContainsKey(targetId))
            _periodicEffects[targetId] = new List<PeriodicEffect>();
        
        _periodicEffects[targetId].Add(periodicEffect);
        UpdatePeriodicTime(targetId, periodicEffect.NextExecutionTime);
    }
    
    /// <summary>
    /// 存储可堆叠周期效果
    /// </summary>
    private void StoreStackablePeriodicEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        var targetId = target.Id;
        var effectKey = $"{effect.Name}_{effect.EffectType}";
        
        if (!_stackableEffects.ContainsKey(targetId))
            _stackableEffects[targetId] = new Dictionary<string, EffectStack>();
            
        if (_stackableEffects[targetId].ContainsKey(effectKey))
        {
            var stack = _stackableEffects[targetId][effectKey];
            stack.CurrentStacks = Math.Min(stack.CurrentStacks + 1, effect.MaxStacks);
        }
        else
        {
            _stackableEffects[targetId][effectKey] = new EffectStack
            {
                EffectKey = effectKey,
                MaxStacks = effect.MaxStacks,
                CurrentStacks = 1
            };
        }
        
        StorePeriodicEffect(effect, target, source, sourceAbilityId);
    }
    
    /// <summary>
    /// 更新过期时间
    /// </summary>
    private void UpdateExpiryTime(Guid targetId, DateTime expiryTime)
    {
        if (!_nextExpiryTimes.ContainsKey(targetId) || _nextExpiryTimes[targetId] > expiryTime)
            _nextExpiryTimes[targetId] = expiryTime;
    }
    
    /// <summary>
    /// 更新周期执行时间
    /// </summary>
    private void UpdatePeriodicTime(Guid targetId, DateTime nextExecutionTime)
    {
        if (!_nextPeriodicTimes.ContainsKey(targetId) || _nextPeriodicTimes[targetId] > nextExecutionTime)
            _nextPeriodicTimes[targetId] = nextExecutionTime;
    }

    /// <summary>
    /// 处理无限效果：应用修改 -> 存储到ActiveEffects -> 无过期时间 -> 发布事件
    /// </summary>
    private bool ProcessInfiniteEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 应用修改
        ApplyEffectModifications(effect, target);
        
        // 存储效果
        if (effect.StackingType != EffectStackingType.NoStack)
        {
            StoreStackableEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreActiveEffect(effect, target, source, sourceAbilityId);
        }
        
        // 发布事件
        PublishEvent(new EffectApplied(target.Id, effect, target));
        return true;
    }

    /// <summary>
    /// 处理周期效果：存储到PeriodicEffects -> 设置执行间隔 -> 发布事件（修改在UpdatePeriodicEffects中应用）
    /// </summary>
    private bool ProcessPeriodicEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 存储周期效果
        if (effect.StackingType != EffectStackingType.NoStack)
        {
            StoreStackablePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StorePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        
        // 发布事件
        PublishEvent(new EffectApplied(target.Id, effect, target));
        return true;
    }

    /// <summary>
    /// 处理持续周期效果：应用修改 -> 存储到ActiveEffects -> 设置过期时间 -> 存储到PeriodicEffects -> 设置周期执行 -> 发布事件
    /// </summary>
    private bool ProcessDurationPeriodicEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 应用修改
        ApplyEffectModifications(effect, target);
        
        // 存储到ActiveEffects
        if (effect.StackingType == EffectStackingType.NoStack)
        {
            StoreActiveEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreStackableEffect(effect, target, source, sourceAbilityId);
        }
        
        // 设置过期时间
        var expiryTime = Time.GetUnixTimeFromSystem() + effect.Duration.TotalTime;
        UpdateExpiryTime(target.Id, DateTime.FromBinary((long)expiryTime));
        
        // 存储到PeriodicEffects
        if (effect.StackingType == EffectStackingType.NoStack)
        {
            StorePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreStackablePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        
        // 设置周期执行时间
        UpdatePeriodicTime(target.Id, DateTime.UtcNow.AddSeconds(effect.IntervalSeconds));
        
        // 发布事件
        PublishEvent(new EffectApplied(target.Id, effect, target));
        return true;
    }

    /// <summary>
    /// 处理无限周期效果：应用修改 -> 存储到ActiveEffects -> 存储到PeriodicEffects -> 设置周期执行 -> 发布事件
    /// </summary>
    private bool ProcessInfinitePeriodicEffect(AttributeEffect effect, AttributeSet target,
        AttributeSet? source, string? sourceAbilityId)
    {
        // 应用修改
        ApplyEffectModifications(effect, target);
        
        // 存储到ActiveEffects
        if (effect.StackingType == EffectStackingType.NoStack)
        {
            StoreActiveEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreStackableEffect(effect, target, source, sourceAbilityId);
        }
        
        // 存储到PeriodicEffects
        if (effect.StackingType == EffectStackingType.NoStack)
        {
            StorePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        else
        {
            StoreStackablePeriodicEffect(effect, target, source, sourceAbilityId);
        }
        
        // 设置周期执行时间
        UpdatePeriodicTime(target.Id, DateTime.UtcNow.AddSeconds(effect.IntervalSeconds));
        
        // 发布事件
        PublishEvent(new EffectApplied(target.Id, effect, target));
        return true;
    }

    private void ApplyEffectModifications(AttributeEffect effect, AttributeSet target)
    {
        try
        {
            // 遍历效果的所有修饰器
            foreach (var modifier in effect.Modifiers)
            {
                // 获取目标属性
                if (modifier == null) continue;
                var attribute = target.GetAttribute(modifier.AttributeType);
                if (attribute == null)
                {
                    // 如果属性不存在，跳过此修饰器
                    GD.PushError(
                        $"[AbilityEffectService] 属性不存在，跳过修饰器: AttributeType={modifier.AttributeType}, EffectId={effect.Id}");
                    continue;
                }

                target.SetAttributeCurrentValue(attribute.AttributeType, modifier,effect.Source);
            }
        }
        catch (Exception)
        {
            // 忽略异常
        }
    }
        
    private void UnapplyEffectModifications(AttributeEffect effect, AttributeSet target)
    {
        try
        {
            // 遍历效果的所有修饰器，以相反的方式撤销修改
            foreach (var modifier in effect.Modifiers)
            {
                // 获取目标属性
                var attribute = target.GetAttribute(modifier.AttributeType);
                if (attribute == null)
                {
                    // 如果属性不存在，跳过此修饰器
                    GD.Print($"[AbilityEffectService] 撤销时属性不存在，跳过修饰器: AttributeType={modifier.AttributeType}, EffectId={effect.Id}");
                    continue;
                }

                // 根据修饰器操作类型撤销修改
                var currentValue = attribute.CurrentValue;
                var newValue = modifier.RevertModifier(attribute.CurrentValue);
                
                //TODO: 撤销修改器逻辑需要重写
                // 应用新值
                target.SetAttributeCurrentValue(attribute.AttributeType, modifier,effect.Source);
                
                // 记录修改器撤销日志
                GD.Print($"[AbilityEffectService] 修饰器已撤销: EffectId={effect.Id}, AttributeType={modifier.AttributeType}, OperationType={modifier.OperationType}, ModifierValue={modifier.Value}, OldValue={currentValue}, NewValue={newValue}");
            }
        }
        catch (Exception)
        {
            // 忽略异常
        }
    }
    
        
    #endregion
        
    #region 嵌套类
        
    /// <summary>
    /// 活跃效果
    /// </summary>
    public class ActiveEffect
    {
        public AttributeEffect Effect { get; set; } = null!;
        public DateTime AppliedTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public AttributeSet? Source { get; set; }
        public string? SourceAbilityId { get; set; }
    }
        
    /// <summary>
    /// 效果叠加信息
    /// </summary>
    public class EffectStack
    {
        public string EffectKey { get; set; } = string.Empty;
        public int MaxStacks { get; set; }
        public int CurrentStacks { get; set; }
    }
    
    /// <summary>
    /// 周期效果信息
    /// </summary>
    public class PeriodicEffect
    {
        public AttributeEffect Effect { get; set; } = null!;
        public DateTime AppliedTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public double IntervalSeconds { get; set; }
        public AttributeSet? Source { get; set; }
        public string? SourceAbilityId { get; set; }
        public int ExecutionCount { get; set; }
    }
    
        
    #endregion
        
    /// <summary>
    /// 刷新效果持续时间
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <param name="target">目标属性集</param>
    /// <param name="newDuration">新的持续时间（可选）</param>
    /// <returns>是否成功刷新</returns>
    public bool RefreshEffect(string effectId, AttributeSet target, TimeSpan? newDuration = null)
    {
        if (string.IsNullOrEmpty(effectId) || target == null)
            return false;
            
        var targetId = target.Id;
            
        return ExecuteWithLock(() =>
        {
            if (!_activeEffects.ContainsKey(targetId))
                return false;
                
            var effects = _activeEffects[targetId];
            var effect = effects.FirstOrDefault(e => e.Effect.Id.ToString() == effectId);
                
            if (effect == null)
                return false;
                
            try
            {
                if (newDuration.HasValue)
                {
                    effect.ExpiryTime = DateTime.UtcNow.Add(newDuration.Value);
                }
                else
                {
                    effect.ExpiryTime = effect.Effect.Duration.IsInfinite ? DateTime.MaxValue : DateTime.UtcNow.AddSeconds(effect.Effect.Duration.TotalTime);
                }
                    
                PublishEvent(new EffectRefreshed(targetId, effect.Effect, target));
                _nextExpiryTimes[targetId] = effects.Min(e => e.ExpiryTime);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        });
    }
        
        
    /// <summary>
    /// 清理所有效果
    /// </summary>
    public void ClearAllEffects()
    {
        ExecuteWithLock(() =>
        {
            try
            {
                _activeEffects.Clear();
                _stackableEffects.Clear();
                _nextExpiryTimes.Clear();
            }
            catch (Exception)
            {
                // 忽略异常
            }
        });
    }
}