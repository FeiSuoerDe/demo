
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性值类，包含基础值、当前值、最小值和最大值
/// 根据GAS设计文档要求，支持完整的属性数值结构
/// </summary>
public class AttributeValue
{
    /// <summary>
    /// 基础值
    /// </summary>
    public float BaseValue { get; private set; }
        
    /// <summary>
    /// 当前值（经过修饰器计算后的值）
    /// </summary>
    public float CurrentValue { get; protected set; }
        
    /// <summary>
    /// 最小值限制
    /// </summary>
    public float MinValue { get; private set; }
        
    /// <summary>
    /// 最大值限制
    /// </summary>
    public float MaxValue { get; private set; }
        
    /// <summary>
    /// 属性类型
    /// </summary>
    public AttributeDefinition AttributeType { get; private set; }
        
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="attributeType">属性类型</param>
    /// <param name="baseValue">基础值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    public AttributeValue(AttributeDefinition attributeType, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        AttributeType = attributeType;
        MinValue = minValue;
        MaxValue = maxValue;
        BaseValue = ClampValue(baseValue);
        CurrentValue = BaseValue;
    }
        
    /// <summary>
    /// 设置基础值
    /// </summary>
    /// <param name="value">新的基础值</param>
    public virtual void SetBaseValue(float value)
    {
        BaseValue = ClampValue(value);
        CurrentValue = BaseValue;
    }
    /// <summary>
    /// 设置当前值
    /// </summary>
    /// <param name="value">新的当前值</param>
    public virtual void SetCurrentValue(float value)
    {
        CurrentValue = ClampValue(value);
    }

    /// <summary>
    /// 自定义计算公式
    /// </summary>
    /// <param name="attributes"></param>
    public virtual float CustomCompute(IEnumerable<AttributeValue?> attributes)
    {
        return CurrentValue;
    }

    public virtual void ComputeValue(IEnumerable<AttributeValue?> attributes, AttributeModifier? modifier,Dictionary<AttributeDefinition, float>? effectSource = null)
    {
        var computeResult = CustomCompute(attributes);
        SetCurrentValue(modifier?.ExecuteModifier(computeResult) ?? computeResult);
    }
        
    /// <summary>
    /// 重置当前值为基础值
    /// </summary>
    public virtual void Reset()
    {
        CurrentValue = BaseValue;
    }
        
    public void SetMaxValue(float maxValue)
    {
        if (MinValue > maxValue)
            throw new ArgumentException("最小值不能大于最大值");
        MaxValue = maxValue;
        // 重新应用限制
        BaseValue = ClampValue(BaseValue);
        CurrentValue = ClampValue(CurrentValue);
    }
        
    public void SetMinValue(float minValue)
    {
        if (minValue > MaxValue)
            throw new ArgumentException("最小值不能大于最大值");
        MinValue = minValue;
        // 重新应用限制
        BaseValue = ClampValue(BaseValue);
        CurrentValue = ClampValue(CurrentValue);
    }
        
    /// <summary>
    /// 限制值在有效范围内
    /// </summary>
    /// <param name="value">要限制的值</param>
    /// <returns>限制后的值</returns>
    private float ClampValue(float value)
    {
        return Math.Max(MinValue, Math.Min(MaxValue, value));
    }
        
    /// <summary>
    /// 获取值的百分比（相对于最大值）
    /// </summary>
    /// <returns>百分比值（0-1）</returns>
    public float GetPercentage()
    {
        if (Math.Abs(MaxValue - MinValue) < float.Epsilon)
            return 1.0f;
                
        return (CurrentValue - MinValue) / (MaxValue - MinValue);
    }

    /// <summary>
    /// 获取此属性所依赖的其他属性类型。
    /// </summary>
    /// <returns>依赖的属性类型列表。</returns>
    public virtual IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return Array.Empty<AttributeDefinition>();
    }

    /// <summary>
    /// 获取受此属性影响的其他属性类型。
    /// </summary>
    /// <returns>受影响的属性类型列表。</returns>
    public virtual IEnumerable<AttributeDefinition> GetAffectedAttributes()
    {
        return Array.Empty<AttributeDefinition>();
    }
        
    /// <summary>
    /// 获取属性值的字符串表示
    /// </summary>
    /// <returns>格式化的属性值字符串</returns>
    public override string ToString()
    {
        return $"{AttributeType.Key}: Base={BaseValue:F2}, Current={CurrentValue:F2}, Range=[{MinValue:F2}, {MaxValue:F2}]";
    }
        
    /// <summary>
    /// 判断两个属性值是否相等
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>是否相等</returns>
    public override bool Equals(object obj)
    {
        if (obj is AttributeValue other)
        {
            return AttributeType == other.AttributeType &&
                   Math.Abs(BaseValue - other.BaseValue) < float.Epsilon &&
                   Math.Abs(CurrentValue - other.CurrentValue) < float.Epsilon;
        }
        return false;
    }
        
    /// <summary>
    /// 获取哈希码
    /// </summary>
    /// <returns>哈希码</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(AttributeType, BaseValue, CurrentValue);
    }
}