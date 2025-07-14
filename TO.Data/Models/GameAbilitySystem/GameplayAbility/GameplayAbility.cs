using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 游戏技能基类，表示一个可以被激活的技能或能力
    /// 根据GAS设计文档要求，支持完整的技能系统功能
    /// </summary>
    public abstract class GameplayAbility
    {
        /// <summary>
        /// 技能唯一标识符
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name { get; protected set; }
        
        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; protected set; }
        
        /// <summary>
        /// 技能标签
        /// </summary>
        public HashSet<string> Tags { get; protected set; }
        
        /// <summary>
        /// 技能类型
        /// </summary>
        public AbilityType AbilityType { get; protected set; }
        
        /// <summary>
        /// 激活策略
        /// </summary>
        public AbilityActivationPolicy ActivationPolicy { get; protected set; }
        
        /// <summary>
        /// 冷却时间
        /// </summary>
        public Duration CooldownDuration { get; protected set; }
        
        /// <summary>
        /// 消耗的属性和数值
        /// </summary>
        public Dictionary<AttributeType, float> Costs { get; protected set; }
        
        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level { get; private set; }
        
        /// <summary>
        /// 最大等级
        /// </summary>
        public int MaxLevel { get; protected set; }
        
        /// <summary>
        /// 技能状态
        /// </summary>
        public AbilityStatus Status { get; private set; }
        
        /// <summary>
        /// 是否正在冷却
        /// </summary>
        public bool IsOnCooldown { get; private set; }
        
        /// <summary>
        /// 冷却剩余时间
        /// </summary>
        public Duration RemainingCooldown { get; private set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; private set; }
        
        /// <summary>
        /// 最后激活时间
        /// </summary>
        public DateTime? LastActivatedTime { get; private set; }
        
        /// <summary>
        /// 技能拥有者ID
        /// </summary>
        public Guid OwnerId { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">技能名称</param>
        /// <param name="description">技能描述</param>
        /// <param name="abilityType">技能类型</param>
        /// <param name="activationPolicy">激活策略</param>
        /// <param name="cooldownDuration">冷却时间</param>
        /// <param name="maxLevel">最大等级</param>
        /// <param name="ownerId">拥有者ID</param>
        protected GameplayAbility(string name, string description, AbilityType abilityType,
            AbilityActivationPolicy activationPolicy, Duration cooldownDuration, int maxLevel = 1, Guid ownerId = default)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            AbilityType = abilityType;
            ActivationPolicy = activationPolicy;
            CooldownDuration = cooldownDuration;
            MaxLevel = maxLevel;
            Level = 1;
            Status = AbilityStatus.Ready;
            IsOnCooldown = false;
            RemainingCooldown = Duration.FromSeconds(0);
            CreatedTime = DateTime.UtcNow;
            OwnerId = ownerId;
            
            Tags = new HashSet<string>();
            Costs = new Dictionary<AttributeType, float>();
        }
        
        /// <summary>
        /// 检查技能是否可以激活
        /// </summary>
        /// <param name="attributeSet">属性集</param>
        /// <returns>是否可以激活</returns>
        public virtual bool CanActivate(AttributeSet attributeSet)
        {
            // 检查技能状态
            if (Status != AbilityStatus.Ready)
                return false;
                
            // 检查冷却时间
            if (IsOnCooldown)
                return false;
                
            // 检查消耗
            foreach (var cost in Costs)
            {
                var currentValue = attributeSet.GetAttribute(cost.Key)!.CurrentValue;
                if (currentValue < cost.Value)
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 激活技能
        /// </summary>
        /// <param name="attributeSet">属性集</param>
        /// <param name="target">目标</param>
        /// <returns>激活结果</returns>
        public async Task<AbilityActivationResult> ActivateAsync(AttributeSet attributeSet, object target = null)
        {
            if (!CanActivate(attributeSet))
            {
                return new AbilityActivationResult(false, "技能无法激活");
            }
            
            try
            {
                Status = AbilityStatus.Activating;
                
                // 消耗资源
                ConsumeResources(attributeSet);
                
                // 执行技能逻辑
                var result = await ExecuteAbilityAsync(attributeSet, target);
                
                // 开始冷却
                StartCooldown();
                
                Status = AbilityStatus.Ready;
                LastActivatedTime = DateTime.UtcNow;
                
                return result;
            }
            catch (Exception ex)
            {
                Status = AbilityStatus.Ready;
                return new AbilityActivationResult(false, $"技能执行失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 执行技能逻辑（由子类实现）
        /// </summary>
        /// <param name="attributeSet">属性集</param>
        /// <param name="target">目标</param>
        /// <returns>执行结果</returns>
        protected abstract Task<AbilityActivationResult> ExecuteAbilityAsync(AttributeSet attributeSet, object target);
        
        /// <summary>
        /// 消耗资源
        /// </summary>
        /// <param name="attributeSet">属性集</param>
        protected virtual void ConsumeResources(AttributeSet attributeSet)
        {
            foreach (var cost in Costs)
            {
                var currentValue = attributeSet.GetAttribute(cost.Key)!.CurrentValue;
                attributeSet.GetAttribute(cost.Key)?.SetCurrentValue(currentValue - cost.Value);
            }
        }
        
        /// <summary>
        /// 开始冷却
        /// </summary>
        protected virtual void StartCooldown()
        {
            if (!CooldownDuration.IsInfinite && CooldownDuration.TotalTime > 0)
            {
                IsOnCooldown = true;
                RemainingCooldown = Duration.FromSeconds(CooldownDuration.TotalTime);
            }
        }
        
        /// <summary>
        /// 更新冷却时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void UpdateCooldown(float deltaTime)
        {
            if (IsOnCooldown)
            {
                RemainingCooldown.Update(deltaTime);
                
                if (RemainingCooldown.IsExpired)
                {
                    IsOnCooldown = false;
                    RemainingCooldown = Duration.FromSeconds(0);
                }
            }
        }
        
        /// <summary>
        /// 升级技能
        /// </summary>
        /// <returns>是否成功升级</returns>
        public virtual bool LevelUp()
        {
            if (Level >= MaxLevel)
                return false;
                
            Level++;
            OnLevelUp();
            return true;
        }
        
        /// <summary>
        /// 技能升级时的回调
        /// </summary>
        protected virtual void OnLevelUp()
        {
            // 子类可以重写此方法来处理升级逻辑
        }
        
        /// <summary>
        /// 添加技能标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                Tags.Add(tag);
            }
        }
        
        /// <summary>
        /// 移除技能标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveTag(string tag)
        {
            return Tags.Remove(tag);
        }
        
        /// <summary>
        /// 检查是否包含指定标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否包含该标签</returns>
        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }
        
        /// <summary>
        /// 设置技能消耗
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <param name="cost">消耗数值</param>
        public void SetCost(AttributeType attributeType, float cost)
        {
            if (cost > 0)
            {
                Costs[attributeType] = cost;
            }
            else
            {
                Costs.Remove(attributeType);
            }
        }
        
        /// <summary>
        /// 获取技能消耗
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>消耗数值</returns>
        public float GetCost(AttributeType attributeType)
        {
            return Costs.TryGetValue(attributeType, out var cost) ? cost : 0f;
        }
        
        /// <summary>
        /// 重置冷却时间
        /// </summary>
        public void ResetCooldown()
        {
            IsOnCooldown = false;
            RemainingCooldown = Duration.FromSeconds(0);
        }
        
        /// <summary>
        /// 取消技能
        /// </summary>
        public virtual void Cancel()
        {
            if (Status == AbilityStatus.Activating)
            {
                Status = AbilityStatus.Ready;
                OnCancel();
            }
        }
        
        /// <summary>
        /// 技能取消时的回调
        /// </summary>
        protected virtual void OnCancel()
        {
            // 子类可以重写此方法来处理取消逻辑
        }
        
        /// <summary>
        /// 获取技能信息的字符串表示
        /// </summary>
        /// <returns>格式化的技能信息</returns>
        public override string ToString()
        {
            var cooldownInfo = IsOnCooldown ? $" (冷却中: {RemainingCooldown.RemainingTime:F1}s)" : "";
            var levelInfo = MaxLevel > 1 ? $" Lv.{Level}/{MaxLevel}" : "";
            return $"{Name}{levelInfo} [{Status}]{cooldownInfo}";
        }
    }
}