using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayEffect;

/// <summary>
/// 属性修饰器类，用于修改属性值
/// 根据GAS设计文档要求，支持完整的修饰器功能
/// </summary>
public class AttributeModifier
{
    public Guid Id { get; }
    public AttributeDefinition AttributeType { get; }
    public ModifierOperationType OperationType { get; }
    public float Value { get; private set; }
    public int ExecutionOrder { get; }
    public int Priority { get; }
    
    public AttributeModifier(AttributeDefinition attributeType, ModifierOperationType operationType, 
        float value, int executionOrder = 0, int priority = 0)
    {
        Id = Guid.NewGuid();
        AttributeType = attributeType;
        OperationType = operationType;
        Value = value;
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
        return new AttributeModifier(AttributeType, OperationType, Value, ExecutionOrder, Priority);
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
        return $"{AttributeType.Key} {operationSymbol} {Value:F2}";
    }
        
    /// <summary>
    /// 根据修饰器类型执行修饰操作
    /// </summary>
    /// <param name="baseValue">基础值</param>
    /// <returns>修饰后的值</returns>
    public float ExecuteModifier(float baseValue)
    {
        return OperationType switch
        {
            ModifierOperationType.Add => baseValue + Value,
            ModifierOperationType.Subtract => baseValue - Value,
            ModifierOperationType.Multiply => baseValue * Value,
            ModifierOperationType.Override => Value,
            ModifierOperationType.Percentage => baseValue * (1 + Value / 100),
            _ => baseValue
        };
    }
    
    
    /// <summary>
    /// 撤销应用修饰器，根据修饰器类型对已修饰的值进行反向计算
    /// 覆盖类型无法简单撤销，需要重新计算基础值
    /// 这里暂时恢复到基础值，实际应用中可能需要更复杂的逻辑
    /// </summary>
    /// <param name="modifiedValue">已修饰的值</param>
    /// <returns>撤销修饰后的值（即基础值）</returns>
    public float RevertModifier(float modifiedValue)
    {
        return OperationType switch
        {
            ModifierOperationType.Add => modifiedValue - Value,
            ModifierOperationType.Subtract => modifiedValue + Value,
            ModifierOperationType.Multiply => Value != 0 ? modifiedValue / Value : modifiedValue,
            ModifierOperationType.Override => modifiedValue, // 无法撤销Override操作，因为原始值已丢失
            ModifierOperationType.Percentage => Math.Abs(Value - -100) > 0 ? modifiedValue / (1 + Value / 100) : modifiedValue,
            _ => modifiedValue
        };
    }
}
