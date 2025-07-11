using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    
    /// <summary>
    /// 属性修饰器类
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
        public float Value { get; }
        
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
        public AttributeModifier(Guid id, AttributeType attributeType, ModifierOperationType operationType, 
                               float value, ModifierSource source, int executionOrder = 0, 
                               bool isTemporary = false, Duration? duration = null)
        {
            Id = id;
            AttributeType = attributeType;
            OperationType = operationType;
            Value = value;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            ExecutionOrder = executionOrder;
            IsTemporary = isTemporary;
            Duration = duration ?? Duration.Infinite;
        }
        
        /// <summary>
        /// 检查修饰器是否已过期
        /// </summary>
        public bool IsExpired => IsTemporary && Duration is { IsExpired: true };
        
        /// <summary>
        /// 更新持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateDuration(float deltaTime)
        {
            if (IsTemporary && Duration is { IsInfinite: false })
            {
                Duration.Update(deltaTime);
            }
        }
        
        /// <summary>
        /// 应用修饰器到基础值
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <returns>修饰后的值</returns>
        public float Apply(float baseValue)
        {
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
            var tempInfo = IsTemporary ? $" (Temp: {Duration})" : "";
            return $"{AttributeType} {OperationType} {Value} from {Source}{tempInfo}";
        }
    }
}