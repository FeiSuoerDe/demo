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
        public Guid Id { get; }
        public AttributeType AttributeType { get; }
        public ModifierOperationType OperationType { get; }
        public float Value { get; private set; }
        public SourceType Source { get; }
        public int ExecutionOrder { get; }
        public int Priority { get; }
    
        public AttributeModifier(AttributeType attributeType, ModifierOperationType operationType, 
                               float value, SourceType source, int executionOrder = 0, int priority = 0)
        {
            Id = Guid.NewGuid();
            AttributeType = attributeType;
            OperationType = operationType;
            Value = value;
            Source = source;
            ExecutionOrder = executionOrder;
            Priority = priority;
    
            ValidateModifierValue();
        }

    
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
    
        public void SetModifierValue(float newValue)
        {
            var oldValue = Value;
            Value = newValue;
            try
            {
                ValidateModifierValue();
            }
            catch
            {
                Value = oldValue;
                throw;
            }
        }
    
        public AttributeModifier Clone()
        {
            return new AttributeModifier(AttributeType, OperationType, Value, Source, ExecutionOrder, Priority);
        }
    
        public override bool Equals(object? obj)
        {
            if (obj is AttributeModifier other)
            {
                return Id == other.Id;
            }
            return false;
        }
    
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    
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
            return $"{AttributeType} {operationSymbol} {Value:F2}";
        }
    }
}