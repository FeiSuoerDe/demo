using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayEffect
{
    
    /// <summary>
    /// 属性修饰器类，用于修改属性值
    /// 根据GAS设计文档要求，支持完整的修饰器功能
    /// </summary>
    public class AttributeModifier
    {
        /// <summary>
        /// 修饰器唯一标识
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// 目标属性类型
        /// </summary>
        public AttributeType AttributeType { get; }
        
        /// <summary>
        /// 操作类型
        /// </summary>
        public ModifierOperationType OperationType { get; }
        
        /// <summary>
        /// 修饰值
        /// </summary>
        public float Value { get; private set; }
        
        /// <summary>
        /// 修饰器来源
        /// </summary>
        public ModifierSource Source { get; }
        
        /// <summary>
        /// 执行顺序（决定修饰器应用顺序）
        /// </summary>
        public int ExecutionOrder { get; }
        
        /// <summary>
        /// 是否为临时修饰器
        /// </summary>
        public bool IsTemporary { get; }
        
        /// <summary>
        /// 持续时间
        /// </summary>
        public Duration? Duration { get; }
        
        /// <summary>
        /// 修饰器优先级
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// 修饰器标签
        /// </summary>
        public string Tag { get; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdatedTime { get; private set; }
        
        /// <summary>
        /// 修饰器状态
        /// </summary>
        public ModifierStatus Status { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">修饰器ID</param>
        /// <param name="attributeType">目标属性类型</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="value">修饰值</param>
        /// <param name="source">修饰器来源</param>
        /// <param name="executionOrder">执行顺序</param>
        /// <param name="isTemporary">是否临时</param>
        /// <param name="duration">持续时间</param>
        /// <param name="priority">修饰器优先级</param>
        /// <param name="tag">修饰器标签</param>
        public AttributeModifier(Guid id, AttributeType attributeType, ModifierOperationType operationType, 
                               float value, ModifierSource source, int executionOrder = 0, 
                               bool isTemporary = false, Duration? duration = null, int priority = 0, string tag = "")
        {
            Id = id;
            AttributeType = attributeType;
            OperationType = operationType;
            Value = value;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            ExecutionOrder = executionOrder;
            IsTemporary = isTemporary;
            Duration = duration ?? Duration.Infinite;
            Priority = priority;
            Tag = tag ?? string.Empty;
            Status = ModifierStatus.Active;
            CreatedTime = DateTime.UtcNow;
            LastUpdatedTime = CreatedTime;
            
            // 验证临时修饰器必须有持续时间
            if (isTemporary && duration == null)
            {
                throw new ArgumentException("临时修饰器必须指定持续时间");
            }
            
            // 验证修饰器值的合理性
            ValidateModifierValue();
        }
        
        /// <summary>
        /// 检查修饰器是否已过期
        /// </summary>
        public bool IsExpired => IsTemporary && Duration is { IsExpired: true } && Status == ModifierStatus.Active;
        
        /// <summary>
        /// 是否为永久修饰器
        /// </summary>
        public bool IsPermanent => !IsTemporary;
        
        /// <summary>
        /// 获取剩余时间百分比
        /// </summary>
        public float RemainingTimePercentage => Duration?.RemainingPercentage ?? 1.0f;
        
        /// <summary>
        /// 更新持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateDuration(float deltaTime)
        {
            if (IsTemporary && Duration is { IsInfinite: false } && Status == ModifierStatus.Active)
            {
                Duration.Update(deltaTime);
                LastUpdatedTime = DateTime.UtcNow;
                
                if (Duration.IsExpired)
                {
                    Status = ModifierStatus.Expired;
                }
            }
        }
        
        /// <summary>
        /// 暂停修饰器
        /// </summary>
        public void Pause()
        {
            if (Status == ModifierStatus.Active)
            {
                Status = ModifierStatus.Paused;
                LastUpdatedTime = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// 恢复修饰器
        /// </summary>
        public void Resume()
        {
            if (Status == ModifierStatus.Paused)
            {
                Status = ModifierStatus.Active;
                LastUpdatedTime = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// 取消修饰器
        /// </summary>
        public void Cancel()
        {
            Status = ModifierStatus.Cancelled;
            LastUpdatedTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 应用修饰器到基础值
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <returns>修饰后的值</returns>
        public float Apply(float baseValue)
        {
            if (Status != ModifierStatus.Active)
                return baseValue;
                
            return OperationType switch
            {
                ModifierOperationType.Add => baseValue + Value,
                ModifierOperationType.Multiply => baseValue * Value,
                ModifierOperationType.Override => Value,
                ModifierOperationType.Percentage => baseValue * (1 + Value / 100),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <summary>
        /// 验证修饰器值的合理性
        /// </summary>
        private void ValidateModifierValue()
        {
            switch (OperationType)
            {
                case ModifierOperationType.Multiply:
                    if (Value < 0)
                        throw new ArgumentException("乘法修饰器值不能为负数");
                    break;
                    
                case ModifierOperationType.Percentage:
                    if (Value < -100 || Value > 1000)
                        throw new ArgumentException("百分比修饰器值应在-100%到1000%之间");
                    break;
            }
        }
        
        /// <summary>
        /// 设置新的修饰值
        /// </summary>
        /// <param name="newValue">新的修饰值</param>
        public void SetModifierValue(float newValue)
        {
            var oldValue = Value;
            Value = newValue;
            LastUpdatedTime = DateTime.UtcNow;
            
            // 重新验证
            try
            {
                ValidateModifierValue();
            }
            catch
            {
                // 如果验证失败，恢复原值
                Value = oldValue;
                throw;
            }
        }
        
        /// <summary>
        /// 创建修饰器的副本
        /// </summary>
        /// <returns>修饰器副本</returns>
        public AttributeModifier Clone()
        {
            return new AttributeModifier(
                Guid.NewGuid(),
                AttributeType,
                OperationType,
                Value,
                Source,
                ExecutionOrder,
                IsTemporary,
                Duration,
                Priority,
                Tag
            );
        }
        
        /// <summary>
        /// 判断两个修饰器是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object? obj)
        {
            if (obj is AttributeModifier other)
            {
                return Id == other.Id;
            }
            return false;
        }
        
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>格式化的修饰器信息字符串</returns>
        public override string ToString()
        {
            var operationSymbol = OperationType switch
            {
                ModifierOperationType.Add => "+",
                ModifierOperationType.Multiply => "×",
                ModifierOperationType.Override => "=",
                ModifierOperationType.Percentage => "%",
                _ => "?"
            };
            
            var statusInfo = Status != ModifierStatus.Active ? $" [{Status}]" : "";
            var durationInfo = IsTemporary ? $" (Duration: {Duration})" : " (Permanent)";
            var tagInfo = !string.IsNullOrEmpty(Tag) ? $" #{Tag}" : "";
            
            return $"{AttributeType} {operationSymbol} {Value:F2}{durationInfo}{tagInfo}{statusInfo}";
        }
    }
}