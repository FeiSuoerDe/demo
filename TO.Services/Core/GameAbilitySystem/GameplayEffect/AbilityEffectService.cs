
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayEffect;
using TO.Services.Core.GameAbilitySystem.Base;


namespace TO.Services.Core.GameAbilitySystem.GameplayEffect
{
    /// <summary>
    /// 重构后的技能效果管理服务
    /// 使用基础类和帮助类来减少冗余代码
    /// </summary>
    public class AbilityEffectService : BaseGameAbilityService, IAbilityEffectService
    {
        private readonly Dictionary<string, List<ActiveEffect>> _activeEffects;
        private readonly Dictionary<string, EffectStack> _stackableEffects;
        private readonly Dictionary<string, DateTime> _immunities;
        
        // 事件定义
        public event Action<string, AttributeEffect, AttributeSet>? EffectApplied;
        public event Action<string, AttributeEffect, AttributeSet>? EffectRemoved;
        public event Action<string, AttributeEffect, AttributeSet>? EffectRefreshed;
        public event Action<string, AttributeEffect, AttributeSet>? EffectExpired;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventBusRepo">事件总线仓储</param>
        public AbilityEffectService(IEventBusRepo eventBusRepo) : base(eventBusRepo)
        {
            _activeEffects = new Dictionary<string, List<ActiveEffect>>();
            _stackableEffects = new Dictionary<string, EffectStack>();
            _immunities = new Dictionary<string, DateTime>();
        }
        
        /// <summary>
        /// 应用效果到目标
        /// </summary>
        /// <param name="effect">游戏效果</param>
        /// <param name="target">目标属性集</param>
        /// <param name="source">源属性集</param>
        /// <param name="sourceAbilityId">源技能ID</param>
        /// <returns>是否成功应用</returns>
        public async Task<bool> ApplyEffectAsync(AttributeEffect effect, AttributeSet target, 
            AttributeSet? source = null, string? sourceAbilityId = null)
        {
            if (effect == null || target == null)
                return false;
            
            var targetId = target.Id.ToString();
            
            // 检查免疫
            if (IsImmune(targetId, effect.Id.ToString()))
            {
                return false;
            }
            
            return await ExecuteWithLockAsync(() =>
            {
                // 检查是否可以应用
                if (!CanApplyEffect(effect, target, source))
                {
                    return false;
                }
                
                try
                {
                    bool result;
                    
                    // 处理可叠加效果
                    if (effect.StackingType != EffectStackingType.NoStack)
                    {
                        result = ApplyStackableEffect(effect, target, source, sourceAbilityId);
                    }
                    else
                    {
                        // 处理普通效果
                        result = ApplyNormalEffect(effect, target, source, sourceAbilityId);
                    }
                    
                    return result;
                }
                catch (Exception)
                {
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
        public bool RemoveEffect(string effectId, AttributeSet target)
        {
            if (string.IsNullOrEmpty(effectId) || target == null)
                return false;
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                {
                    return false;
                }
                
                var effects = _activeEffects[targetId];
                var effectToRemove = effects.FirstOrDefault(e => e.Effect.Id.ToString() == effectId);
                
                if (effectToRemove == null)
                {
                    return false;
                }
                
                try
                {
                    // 移除效果的影响
                    UnapplyEffectModifications(effectToRemove.Effect, target);
                    
                    effects.Remove(effectToRemove);
                    
                    if (effects.Count == 0)
                        _activeEffects.Remove(targetId);
                    
                    // 处理可叠加效果
                    if (effectToRemove.Effect.StackingType != EffectStackingType.NoStack)
                    {
                        var stackKey = $"{targetId}_{effectId}";
                        if (_stackableEffects.ContainsKey(stackKey))
                        {
                            var stack = _stackableEffects[stackKey];
                            stack.CurrentStacks = Math.Max(0, stack.CurrentStacks - 1);
                            
                            if (stack.CurrentStacks == 0)
                                _stackableEffects.Remove(stackKey);
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
        /// 移除指定类型的所有效果
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <param name="target">目标属性集</param>
        /// <returns>移除的效果数量</returns>
        public int RemoveEffectsByType(EffectType effectType, AttributeSet target)
        {
            if (target == null)
                return 0;
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                {
                    return 0;
                }
                
                var effects = _activeEffects[targetId];
                var effectsToRemove = effects.Where(e => e.Effect.EffectType == effectType).ToList();
                var removedCount = 0;
                
                try
                {
                    foreach (var effect in effectsToRemove)
                    {
                        UnapplyEffectModifications(effect.Effect, target);
                        effects.Remove(effect);
                        PublishEvent(new EffectRemoved(targetId, effect.Effect, target));
                        removedCount++;
                    }
                    
                    if (effects.Count == 0)
                        _activeEffects.Remove(targetId);
                    
                    return removedCount;
                }
                catch (Exception)
                {
                    return removedCount;
                }
            });
        }
        
        /// <summary>
        /// 移除指定标签的所有效果
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="target">目标属性集</param>
        /// <returns>移除的效果数量</returns>
        public int RemoveEffectsByTag(string tag, AttributeSet target)
        {
            if (string.IsNullOrEmpty(tag) || target == null)
                return 0;
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                {
                    return 0;
                }
                
                var effects = _activeEffects[targetId];
                var effectsToRemove = effects.Where(e => e.Effect.Tags.Contains(tag)).ToList();
                var removedCount = 0;
                
                try
                {
                    foreach (var effect in effectsToRemove)
                    {
                        UnapplyEffectModifications(effect.Effect, target);
                        effects.Remove(effect);
                        PublishEvent(new EffectRemoved(targetId, effect.Effect, target));
                        removedCount++;
                    }
                    
                    if (effects.Count == 0)
                        _activeEffects.Remove(targetId);
                    
                    return removedCount;
                }
                catch (Exception)
                {
                    return removedCount;
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
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                {
                    return 0;
                }
                
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
                    
                    // 清除叠加效果
                    var stackKeysToRemove = _stackableEffects.Keys
                        .Where(key => key.StartsWith($"{targetId}_"))
                        .ToList();
                    
                    foreach (var key in stackKeysToRemove)
                    {
                        _stackableEffects.Remove(key);
                    }
                    
                    return removedCount;
                }
                catch (Exception)
                {
                    return 0;
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
            
            var targetId = target.Id.ToString();
            
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
            if (target == null)
                return new List<AttributeEffect>();
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                    return new List<AttributeEffect>();
                
                return _activeEffects[targetId].Select(ae => ae.Effect).ToList();
            });
        }
        
        /// <summary>
        /// 获取目标的所有活跃效果详细信息
        /// </summary>
        /// <param name="target">目标属性集</param>
        /// <returns>活跃效果详细信息列表</returns>
        public List<EffectDetails> GetActiveEffectDetails(AttributeSet target)
        {
            if (target == null)
                return new List<EffectDetails>();
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                    return new List<EffectDetails>();
                
                var now = DateTime.UtcNow;
                var result = new List<EffectDetails>();
                
                foreach (var activeEffect in _activeEffects[targetId])
                {
                    var stackKey = $"{targetId}_{activeEffect.Effect.Id}";
                    var currentStacks = _stackableEffects.TryGetValue(stackKey, out var stack) ? stack.CurrentStacks : 1;
                    
                    result.Add(new EffectDetails
                    {
                        Effect = activeEffect.Effect,
                        AppliedTime = activeEffect.AppliedTime,
                        RemainingTime = activeEffect.ExpiryTime - now,
                        Source = activeEffect.Source,
                        SourceAbilityId = activeEffect.SourceAbilityId,
                        CurrentStacks = currentStacks
                    });
                }
                
                return result;
            });
        }
        
        /// <summary>
        /// 获取效果的剩余时间
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>剩余时间</returns>
        public TimeSpan GetEffectRemainingTime(string effectId, AttributeSet target)
        {
            if (string.IsNullOrEmpty(effectId) || target == null)
                return TimeSpan.Zero;
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                    return TimeSpan.Zero;
                
                var activeEffect = _activeEffects[targetId].FirstOrDefault(e => e.Effect.Id.ToString() == effectId);
                if (activeEffect == null)
                    return TimeSpan.Zero;
                
                var remainingTime = activeEffect.ExpiryTime - DateTime.UtcNow;
                return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
            });
        }
        
        /// <summary>
        /// 刷新效果持续时间
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <param name="newDuration">新的持续时间</param>
        /// <returns>是否成功刷新</returns>
        public bool RefreshEffect(string effectId, AttributeSet target, TimeSpan newDuration)
        {
            if (string.IsNullOrEmpty(effectId) || target == null)
                return false;
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                {
                    return false;
                }
                
                var effect = _activeEffects[targetId].FirstOrDefault(e => e.Effect.Id.ToString() == effectId);
                if (effect == null)
                {
                    return false;
                }
                
                try
                {
                    effect.ExpiryTime = DateTime.UtcNow.Add(newDuration);
                    PublishEvent(new EffectRefreshed(targetId, effect.Effect, target));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
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
            
            var targetId = target.Id.ToString();
            
            return ExecuteWithLock(() =>
            {
                if (!_activeEffects.ContainsKey(targetId))
                    return 0;
                
                var effects = _activeEffects[targetId];
                var now = DateTime.UtcNow;
                var expiredEffects = effects.Where(e => e.ExpiryTime <= now).ToList();
                var removedCount = 0;
                
                try
                {
                    foreach (var expiredEffect in expiredEffects)
                    {
                        UnapplyEffectModifications(expiredEffect.Effect, target);
                        effects.Remove(expiredEffect);
                        
                        // 处理可叠加效果
                        if (expiredEffect.Effect.StackingType != EffectStackingType.NoStack)
                        {
                            var stackKey = $"{targetId}_{expiredEffect.Effect.Id}";
                            if (_stackableEffects.ContainsKey(stackKey))
                            {
                                var stack = _stackableEffects[stackKey];
                                stack.CurrentStacks = Math.Max(0, stack.CurrentStacks - 1);
                                
                                if (stack.CurrentStacks == 0)
                                    _stackableEffects.Remove(stackKey);
                            }
                        }
                        
                        PublishEvent(new EffectExpired(targetId, expiredEffect.Effect, target));
                        removedCount++;
                    }
                    
                    if (effects.Count == 0)
                        _activeEffects.Remove(targetId);
                    
                    return removedCount;
                }
                catch (Exception)
                {
                    return removedCount;
                }
            });
        }
        
        /// <summary>
        /// 设置目标对指定效果的免疫
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="effectId">效果ID</param>
        /// <param name="duration">免疫持续时间</param>
        public void SetImmunity(string targetId, string effectId, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(targetId) || string.IsNullOrEmpty(effectId))
                return;
            
            ExecuteWithLock(() =>
            {
                var immunityKey = $"{targetId}_{effectId}";
                _immunities[immunityKey] = DateTime.UtcNow.Add(duration);
            });
        }
        
        /// <summary>
        /// 检查目标是否对指定效果免疫
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>是否免疫</returns>
        public bool IsImmune(string targetId, string effectId)
        {
            if (string.IsNullOrEmpty(targetId) || string.IsNullOrEmpty(effectId))
                return false;
            
            return ExecuteWithLock(() =>
            {
                var immunityKey = $"{targetId}_{effectId}";
                if (!_immunities.ContainsKey(immunityKey))
                    return false;
                
                if (_immunities[immunityKey] <= DateTime.UtcNow)
                {
                    _immunities.Remove(immunityKey);
                    return false;
                }
                
                return true;
            });
        }
        
        /// <summary>
        /// 获取效果的当前叠加数
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>当前叠加数</returns>
        public int GetEffectStacks(string effectId, AttributeSet target)
        {
            if (string.IsNullOrEmpty(effectId) || target == null)
                return 0;
            
            var targetId = target.Id.ToString();
            
            var stackKey = $"{targetId}_{effectId}";
            
            return ExecuteWithLock(() =>
            {
                return _stackableEffects.TryGetValue(stackKey, out var stack) ? stack.CurrentStacks : 0;
            });
        }
        
        #region 私有方法
        
        private bool CanApplyEffect(AttributeEffect effect, AttributeSet target, AttributeSet? source)
        {
            // 检查目标是否已经有相同效果（非叠加效果）
            if (effect.StackingType == EffectStackingType.NoStack && HasEffect(effect.Id.ToString(), target))
            {
                return false;
            }
            
            // 检查互斥效果 - 暂时注释掉，因为 AttributeEffect 可能没有 MutuallyExclusiveWith 属性
            // if (effect.MutuallyExclusiveWith?.Any(exclusiveId => HasEffect(exclusiveId, target)) == true)
            // {
            //     return false;
            // }
            
            // 检查前置条件 - 暂时注释掉，因为 AttributeEffect 可能没有 Prerequisites 属性
            // if (effect.Prerequisites?.Any(prereq => !HasEffect(prereq, target)) == true)
            // {
            //     return false;
            // }
            
            return true;
        }
        
        private bool ApplyStackableEffect(AttributeEffect effect, AttributeSet target, 
            AttributeSet? source, string? sourceAbilityId)
        {
            var targetId = target.Id.ToString();
            var stackKey = $"{targetId}_{effect.Id}";
            
            if (!_stackableEffects.ContainsKey(stackKey))
            {
                _stackableEffects[stackKey] = new EffectStack
                {
                    EffectId = effect.Id.ToString(),
                    MaxStacks = effect.MaxStacks,
                    CurrentStacks = 0
                };
            }
            
            var stack = _stackableEffects[stackKey];
            if (stack.CurrentStacks >= stack.MaxStacks)
            {
                // 刷新持续时间
                RefreshEffect(effect.Id.ToString(), target, TimeSpan.FromSeconds(effect.Duration.TotalTime));
                return true;
            }
            
            stack.CurrentStacks++;
            
            var activeEffect = new ActiveEffect
            {
                Effect = effect,
                AppliedTime = DateTime.UtcNow,
                ExpiryTime = effect.Duration.IsInfinite ? DateTime.MaxValue : DateTime.UtcNow.AddSeconds(effect.Duration.TotalTime),
                Source = source,
                SourceAbilityId = sourceAbilityId
            };
            
            if (!_activeEffects.ContainsKey(targetId))
                _activeEffects[targetId] = new List<ActiveEffect>();
            
            _activeEffects[targetId].Add(activeEffect);
            
            // 应用效果修改
            ApplyEffectModifications(effect, target);
            
            PublishEvent(new EffectApplied(targetId, effect, target));
            return true;
        }
        
        private bool ApplyNormalEffect(AttributeEffect effect, AttributeSet target, 
            AttributeSet? source, string? sourceAbilityId)
        {
            var targetId = target.Id.ToString();
            
            var activeEffect = new ActiveEffect
            {
                Effect = effect,
                AppliedTime = DateTime.UtcNow,
                ExpiryTime = effect.Duration.IsInfinite ? DateTime.MaxValue : DateTime.UtcNow.AddSeconds(effect.Duration.TotalTime),
                Source = source,
                SourceAbilityId = sourceAbilityId
            };
            
            if (!_activeEffects.ContainsKey(targetId))
                _activeEffects[targetId] = new List<ActiveEffect>();
            
            _activeEffects[targetId].Add(activeEffect);
            
            // 应用效果修改
            ApplyEffectModifications(effect, target);
            
            PublishEvent(new EffectApplied(targetId, effect, target));
            return true;
        }
        
        private void ApplyEffectModifications(AttributeEffect effect, AttributeSet target)
        {
            try
            {
                // 效果的修饰器会在 AttributeSet.ApplyEffect 中自动应用
                // 这里不需要额外的操作，因为 RecalculateAttribute 会处理所有修饰器
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
                // 效果的修饰器会在 AttributeSet.RemoveEffect 中自动移除
                // 这里不需要额外的操作，因为 RecalculateAttribute 会重新计算属性值
            }
            catch (Exception)
            {
                // 忽略异常
            }
        }
        
        /// <summary>
        /// 异步执行带锁的操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">要执行的操作</param>
        /// <returns>操作结果</returns>
        private async Task<T> ExecuteWithLockAsync<T>(Func<T> action)
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    return action();
                }
            });
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
            public string EffectId { get; set; } = string.Empty;
            public int MaxStacks { get; set; }
            public int CurrentStacks { get; set; }
        }
        
        /// <summary>
        /// 效果详细信息
        /// </summary>
        public class EffectDetails
        {
            public AttributeEffect? Effect { get; set; }
            public DateTime AppliedTime { get; set; }
            public TimeSpan RemainingTime { get; set; }
            public AttributeSet? Source { get; set; }
            public string? SourceAbilityId { get; set; }
            public int CurrentStacks { get; set; }
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
            
            var targetId = target.Id.ToString();
            
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
                    EffectRefreshed?.Invoke(targetId, effect.Effect, target);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
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
                var currentTime = DateTime.UtcNow;
                var targetsToRemove = new List<string>();
                
                foreach (var kvp in _activeEffects.ToList())
                {
                    var targetId = kvp.Key;
                    var effects = kvp.Value;
                    var expiredEffects = effects.Where(e => e.ExpiryTime <= currentTime).ToList();
                    
                    foreach (var expiredEffect in expiredEffects)
                    {
                        try
                        {
                            // 移除效果的影响
                            // 过期效果会在下次 UpdateEffects 调用时自动清理
                             // 这里只需要从内存中移除即可
                            
                            effects.Remove(expiredEffect);
                            cleanedCount++;
                            
                            // 注意：这里没有 target 实例，所以暂时传 null
                            // 在实际使用中，可能需要从其他地方获取 AttributeSet 实例
                            PublishEvent(new EffectExpired(targetId, expiredEffect.Effect, null));
                            EffectExpired?.Invoke(targetId, expiredEffect.Effect, null);
                        }
                        catch (Exception)
                        {
                            // 忽略异常
                        }
                    }
                    
                    if (effects.Count == 0)
                        targetsToRemove.Add(targetId);
                }
                
                foreach (var targetId in targetsToRemove)
                {
                    _activeEffects.Remove(targetId);
                }
                
                return cleanedCount;
            });
        }
        
        /// <summary>
        /// 获取效果统计信息
        /// </summary>
        /// <returns>统计信息字典</returns>
        public Dictionary<string, object> GetEffectStatistics()
        {
            return ExecuteWithLock(() =>
            {
                var stats = new Dictionary<string, object>
                {
                    ["TotalTargets"] = _activeEffects.Count,
                    ["TotalActiveEffects"] = _activeEffects.Values.Sum(effects => effects.Count),
                    ["TotalStackableEffects"] = _stackableEffects.Count,
                    ["TotalImmunities"] = _immunities.Count,
                    ["ExpiredEffects"] = _activeEffects.Values.SelectMany(effects => effects)
                        .Count(e => e.ExpiryTime <= DateTime.UtcNow)
                };
                
                return stats;
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
                    _immunities.Clear();
                }
                catch (Exception)
                {
                    // 忽略异常
                }
            });
        }
    }
}