using System.Collections.Generic;
using System.Linq;
using TO.Data.Attributes;
using TO.Data.Factories;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute.CharacterAttributes;

/// <summary>
/// 代表最大生命值属性。
/// 该值受“体质”(Constitution)属性影响。
/// </summary>
[AttributeValueProvider(GameAttributes.MaxHealthKey)]
public class MaxHealthAttributeValue : AttributeValue
{
    /// <summary>
    /// 每点体质(Constitution)可以增加的生命值。
    /// </summary>
    private const float Strength = 100f;

    /// <summary>
    /// 存储基础生命值，这个值不受任何其他属性（如体质）的影响。
    /// </summary>
    private float _rawBaseValue;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="attributeType">属性定义</param>
    /// <param name="baseValue">基础值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    public MaxHealthAttributeValue(AttributeDefinition attributeType, float baseValue, float minValue, float maxValue)
        : base(attributeType, baseValue, minValue, maxValue)
    {
        // 初始化原始基础值
        _rawBaseValue = baseValue;
    }

    /// <summary>
    /// 设置属性的原始基础值。
    /// 这个值是在计算任何依赖项（如体质加成）之前的数值。
    /// </summary>
    /// <param name="value">新的原始基础值</param>
    public override void SetBaseValue(float value)
    {
        _rawBaseValue = value;
        // 实际的 BaseValue 将在下一次 ComputeValue 调用期间根据依赖关系重新计算。
        // 在此之前，我们调用基类实现以保持外部状态的一致性。
        base.SetBaseValue(value);
    }

    /// <summary>
    /// 根据依赖的属性（如体质）和效果修改器，重新计算最终的属性值。
    /// </summary>
    /// <param name="attributes">当前属性集中的所有属性值，用于查找依赖项。</param>
    /// <param name="modifier">要应用的属性修改器。</param>
    public override void ComputeValue(IEnumerable<AttributeValue?> attributes, AttributeModifier? modifier, Dictionary<AttributeDefinition, float>? effectSource = null)
    {
        // 1. 从依赖属性（体质）计算新的基础值
        var constitution = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.Constitution);
        
        // 如果找到了体质属性，则用它来调整基础生命值；否则，体质加成为0。
        float newBaseValue = _rawBaseValue + (constitution?.CurrentValue ?? 0) * Strength;
        base.SetBaseValue(newBaseValue);

        // 2. 执行任何来自基类的自定义计算（如果存在）
        var computedValue = CustomCompute(attributes);

        // 3. 应用效果修改器（如增益/减益效果）
        var modifiedValue = modifier?.ExecuteModifier(computedValue) ?? computedValue;
        
        // 4. 设置最终的当前值
        SetCurrentValue((int)modifiedValue);
    }

    /// <summary>
    /// 声明此属性依赖于其他哪些属性。
    /// MaxHealth 依赖于 Constitution。
    /// </summary>
    /// <returns>依赖的属性定义列表。</returns>
    public override IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return [GameAttributes.Constitution];
    }
}
