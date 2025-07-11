using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TO.Commons.Enums.Game;
using TO.Commons.Enums;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 属性管理器
    /// 提供高级的属性管理功能，包括属性集管理、效果应用、批量操作等
    /// </summary>
    public class AttributeManager
    {
        private readonly Dictionary<Guid, AttributeSet> _attributeSets;
        private readonly AttributeCalculationEngine _calculationEngine;
        private readonly List<AttributeEffect?> _globalEffects;
        
        /// <summary>
        /// 属性变化事件
        /// </summary>
        public event Action<Guid, AttributeType, float, float> AttributeChanged;
        
        /// <summary>
        /// 效果应用事件
        /// </summary>
        public event Action<Guid, AttributeEffect?> EffectApplied;
        
        /// <summary>
        /// 效果移除事件
        /// </summary>
        public event Action<Guid, AttributeEffect> EffectRemoved;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public AttributeManager()
        {
            _attributeSets = new Dictionary<Guid, AttributeSet>();
            _calculationEngine = AttributeCalculationEngine.Instance;
            _globalEffects = new List<AttributeEffect?>();
        }
        
        /// <summary>
        /// 注册属性集
        /// </summary>
        /// <param name="attributeSet">属性集</param>
        public void RegisterAttributeSet(AttributeSet attributeSet)
        {
            if (attributeSet == null)
                throw new ArgumentNullException(nameof(attributeSet));
            
            if (_attributeSets.ContainsKey(attributeSet.Id))
                throw new InvalidOperationException($"属性集 {attributeSet.Id} 已经注册");
            
            _attributeSets[attributeSet.Id] = attributeSet;
            
            // 订阅属性变化事件
            attributeSet.AttributeChanged += (attrType, oldValue, newValue) =>
                AttributeChanged?.Invoke(attributeSet.Id, attrType, oldValue, newValue);
        }
        
        /// <summary>
        /// 注销属性集
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        public void UnregisterAttributeSet(Guid attributeSetId)
        {
            if (_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
            {
                // 移除所有应用到该属性集的效果
                var effectsToRemove = attributeSet.GetAppliedEffects().ToList();
                foreach (var effect in effectsToRemove)
                {
                    if (effect != null)
                        attributeSet.RemoveEffect(effect.Id);
                }
                
                _attributeSets.Remove(attributeSetId);
            }
        }
        
        /// <summary>
        /// 获取属性集
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>属性集</returns>
        public AttributeSet GetAttributeSet(Guid attributeSetId)
        {
            _attributeSets.TryGetValue(attributeSetId, out var attributeSet);
            return attributeSet;
        }
        
        /// <summary>
        /// 获取所有属性集
        /// </summary>
        /// <returns>属性集列表</returns>
        public IEnumerable<AttributeSet> GetAllAttributeSets()
        {
            return _attributeSets.Values.ToList();
        }
        
        /// <summary>
        /// 应用效果到指定属性集
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effect">效果</param>
        /// <returns>是否成功应用</returns>
        public bool ApplyEffect(Guid attributeSetId, AttributeEffect? effect)
        {
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return false;
            
            var success = attributeSet.ApplyEffect(effect);
            if (success)
            {
                EffectApplied?.Invoke(attributeSetId, effect);
            }
            
            return success;
        }
        
        /// <summary>
        /// 从指定属性集移除效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveEffect(Guid attributeSetId, Guid effectId)
        {
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return false;
            
            var effect = attributeSet.GetAppliedEffects().FirstOrDefault(e => e?.Id == effectId);
            var success = attributeSet.RemoveEffect(effectId);
            
            if (success && effect != null)
            {
                EffectRemoved?.Invoke(attributeSetId, effect);
            }
            
            return success;
        }
        
        /// <summary>
        /// 应用全局效果（影响所有属性集）
        /// </summary>
        /// <param name="effect">全局效果</param>
        public void ApplyGlobalEffect(AttributeEffect? effect)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));
            
            _globalEffects.Add(effect);
            
            // 应用到所有已注册的属性集
            foreach (var attributeSet in _attributeSets.Values)
            {
                attributeSet.ApplyEffect(effect);
                EffectApplied?.Invoke(attributeSet.Id, effect);
            }
        }
        
        /// <summary>
        /// 移除全局效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        public void RemoveGlobalEffect(Guid effectId)
        {
            var effect = _globalEffects.FirstOrDefault(e => e?.Id == effectId);
            if (effect == null)
                return;
            
            _globalEffects.Remove(effect);
            
            // 从所有属性集中移除
            foreach (var attributeSet in _attributeSets.Values)
            {
                if (attributeSet.RemoveEffect(effectId))
                {
                    EffectRemoved?.Invoke(attributeSet.Id, effect);
                }
            }
        }
        
        /// <summary>
        /// 批量应用效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effects">效果列表</param>
        /// <returns>成功应用的效果数量</returns>
        public int ApplyEffects(Guid attributeSetId, IEnumerable<AttributeEffect?> effects)
        {
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return 0;
            
            int successCount = 0;
            foreach (var effect in effects)
            {
                if (attributeSet.ApplyEffect(effect))
                {
                    successCount++;
                    EffectApplied?.Invoke(attributeSetId, effect);
                }
            }
            
            return successCount;
        }
        
        /// <summary>
        /// 批量移除效果
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <param name="effectIds">效果ID列表</param>
        /// <returns>成功移除的效果数量</returns>
        public int RemoveEffects(Guid attributeSetId, IEnumerable<Guid> effectIds)
        {
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return 0;
            
            int successCount = 0;
            foreach (var effectId in effectIds)
            {
                var effect = attributeSet.GetAppliedEffects().FirstOrDefault(e => e?.Id == effectId);
                if (attributeSet.RemoveEffect(effectId))
                {
                    successCount++;
                    if (effect != null)
                    {
                        EffectRemoved?.Invoke(attributeSetId, effect);
                    }
                }
            }
            
            return successCount;
        }
        
        /// <summary>
        /// 更新所有属性集的效果持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public void UpdateEffectDurations(float deltaTime)
        {
            var expiredEffects = new List<(Guid attributeSetId, AttributeEffect effect)>();
            
            foreach (var kvp in _attributeSets)
            {
                var attributeSet = kvp.Value;
                var effects = attributeSet.GetAppliedEffects().ToList();
                
                foreach (var effect in effects)
                {
                    if (effect != null)
                    {
                        effect.UpdateDuration(deltaTime);
                        
                        if (effect.IsExpired)
                        {
                            expiredEffects.Add((kvp.Key, effect));
                        }
                    }
                }
            }
            
            // 移除过期的效果
            foreach (var (attributeSetId, effect) in expiredEffects)
            {
                RemoveEffect(attributeSetId, effect.Id);
            }
            
            // 更新全局效果
            var expiredGlobalEffects = _globalEffects.Where(e => e != null && e.IsExpired).ToList();
            foreach (var effect in expiredGlobalEffects)
            {
                RemoveGlobalEffect(effect.Id);
            }
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
        public AttributeValue GetAttributeValue(Guid attributeSetId, AttributeType attributeType)
        {
            return _attributeSets.TryGetValue(attributeSetId, out var attributeSet) 
                ? attributeSet.GetAttribute(attributeType) 
                : null;
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
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return false;
            
            attributeSet.SetAttribute(attributeType, value);
            return true;
        }
        
        /// <summary>
        /// 查找具有指定效果的属性集
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>具有该效果的属性集列表</returns>
        public IEnumerable<AttributeSet> FindAttributeSetsWithEffect(Guid effectId)
        {
            return _attributeSets.Values.Where(attributeSet => 
                attributeSet.GetAppliedEffects().Any(effect => effect?.Id == effectId));
        }
        
        /// <summary>
        /// 查找具有指定标签的效果
        /// </summary>
        /// <param name="tag">效果标签</param>
        /// <returns>具有该标签的效果列表</returns>
        public IEnumerable<(Guid attributeSetId, AttributeEffect effect)> FindEffectsWithTag(EffectTags tag)
        {
            var results = new List<(Guid, AttributeEffect)>();
            
            foreach (var kvp in _attributeSets)
            {
                var effects = kvp.Value.GetAppliedEffects().Where(e => e != null && e.Tags.HasFlag(tag));
                foreach (var effect in effects)
                {
                    results.Add((kvp.Key, effect));
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
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return null;
            
            var statistics = new AttributeStatistics
            {
                AttributeSetId = attributeSetId,
                TotalAttributes = attributeSet.GetAllAttributes().Count(),
                ActiveEffects = attributeSet.GetAppliedEffects().Count(),
                TotalModifiers = attributeSet.GetAppliedEffects().Where(e => e != null).SelectMany(e => e.Modifiers).Count()
            };
            
            // 计算属性值分布
            var attributes = attributeSet.GetAllAttributes().Values.ToList();
            if (attributes.Any())
            {
                statistics.MinAttributeValue = attributes.Min(a => a.CurrentValue);
                statistics.MaxAttributeValue = attributes.Max(a => a.CurrentValue);
                statistics.AverageAttributeValue = attributes.Average(a => a.CurrentValue);
            }
            
            return statistics;
        }
        
        /// <summary>
        /// 清理过期效果
        /// </summary>
        /// <returns>清理的效果数量</returns>
        public int CleanupExpiredEffects()
        {
            int cleanedCount = 0;
            
            foreach (var attributeSet in _attributeSets.Values)
            {
                var expiredEffects = attributeSet.GetAppliedEffects().Where(e => e != null && e.IsExpired).ToList();
                foreach (var effect in expiredEffects)
                {
                    if (attributeSet.RemoveEffect(effect.Id))
                    {
                        cleanedCount++;
                        EffectRemoved?.Invoke(attributeSet.Id, effect);
                    }
                }
            }
            
            // 清理全局效果
            var expiredGlobalEffects = _globalEffects.Where(e => e != null && e.IsExpired).ToList();
            foreach (var effect in expiredGlobalEffects)
            {
                _globalEffects.Remove(effect);
                cleanedCount++;
            }
            
            return cleanedCount;
        }
        
        /// <summary>
        /// 重置属性集到初始状态
        /// </summary>
        /// <param name="attributeSetId">属性集ID</param>
        /// <returns>是否成功重置</returns>
        public bool ResetAttributeSet(Guid attributeSetId)
        {
            if (!_attributeSets.TryGetValue(attributeSetId, out var attributeSet))
                return false;
            
            // 移除所有效果
            var effects = attributeSet.GetAppliedEffects().ToList();
            foreach (var effect in effects)
            {
                if (effect != null)
                    attributeSet.RemoveEffect(effect.Id);
                EffectRemoved?.Invoke(attributeSetId, effect);
            }
            
            // 属性值会在移除效果时自动重新计算
            
            return true;
        }
        
        /// <summary>
        /// 获取管理器状态信息
        /// </summary>
        /// <returns>管理器状态</returns>
        public ManagerStatus GetStatus()
        {
            return new ManagerStatus
            {
                RegisteredAttributeSets = _attributeSets.Count,
                GlobalEffects = _globalEffects.Count,
                TotalActiveEffects = _attributeSets.Values.Sum(a => a.GetAppliedEffects().Count()),
                TotalModifiers = _attributeSets.Values
                    .SelectMany(a => a.GetAppliedEffects())
                    .Where(e => e != null)
                    .SelectMany(e => e.Modifiers)
                    .Count()
            };
        }
    }
    
    /// <summary>
    /// 属性统计信息
    /// </summary>
    public class AttributeStatistics
    {
        /// <summary>
        /// 属性集ID
        /// </summary>
        public Guid AttributeSetId { get; set; }
        
        /// <summary>
        /// 总属性数量
        /// </summary>
        public int TotalAttributes { get; set; }
        
        /// <summary>
        /// 活跃效果数量
        /// </summary>
        public int ActiveEffects { get; set; }
        
        /// <summary>
        /// 总修饰器数量
        /// </summary>
        public int TotalModifiers { get; set; }
        
        /// <summary>
        /// 最小属性值
        /// </summary>
        public float MinAttributeValue { get; set; }
        
        /// <summary>
        /// 最大属性值
        /// </summary>
        public float MaxAttributeValue { get; set; }
        
        /// <summary>
        /// 平均属性值
        /// </summary>
        public float AverageAttributeValue { get; set; }
    }
    
    /// <summary>
    /// 管理器状态
    /// </summary>
    public class ManagerStatus
    {
        /// <summary>
        /// 已注册的属性集数量
        /// </summary>
        public int RegisteredAttributeSets { get; set; }
        
        /// <summary>
        /// 全局效果数量
        /// </summary>
        public int GlobalEffects { get; set; }
        
        /// <summary>
        /// 总活跃效果数量
        /// </summary>
        public int TotalActiveEffects { get; set; }
        
        /// <summary>
        /// 总修饰器数量
        /// </summary>
        public int TotalModifiers { get; set; }
    }
}