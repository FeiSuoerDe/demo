using TO.Data.Attributes;
using TO.Data.Factories;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute.CharacterAttributes;

/// <summary>
/// 代表生命值属性，继承自AttributeValue，并提供特定的行为。
/// </summary>
[AttributeValueProvider(GameAttributes.HealthKey)]
public class HealthAttributeValue(AttributeDefinition attributeType, float baseValue, float minValue, float maxValue)
    : AttributeValue(attributeType, baseValue, minValue, maxValue)
{
    // 保存上一次的最大生命值，用于计算生命值百分比
    private float? _previousMaxHealth;

    public override float CustomCompute(IEnumerable<AttributeValue?> attributes)
    {
        var maxHealthAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxHealth);
        if (maxHealthAttr == null)
        {
            return CurrentValue;
        }

        var currentMaxHealth = maxHealthAttr.CurrentValue;
        _previousMaxHealth ??= currentMaxHealth;
        
        // 如果最大生命值发生变化，按比例调整当前生命值
        if (Math.Abs(_previousMaxHealth.Value - currentMaxHealth) > float.Epsilon)
        {
            var percentage = _previousMaxHealth.Value == 0 ? 1.0f : CurrentValue / _previousMaxHealth.Value;
            return percentage * currentMaxHealth;
        }

        return CurrentValue;
    }

    public override void ComputeValue(IEnumerable<AttributeValue?> attributes, AttributeModifier? modifier,Dictionary<AttributeDefinition, float>? effectSourc = null)
    {
        var maxHealthAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxHealth);
        var currentMaxHealth = maxHealthAttr?.CurrentValue ?? CurrentValue;

        // 1. 通过CustomCompute获取基于依赖计算的当前值
        var computedValue = CustomCompute(attributes);
        
        // 2. 应用修改器
        var modifiedValue = modifier?.ExecuteModifier(computedValue) ?? computedValue;

        // 3. 确保值在有效范围内并转换为整数
        SetCurrentValue((int)Math.Clamp(modifiedValue, 0, currentMaxHealth));
        
        // 4. 更新上一次的最大生命值以备下次计算
        _previousMaxHealth = currentMaxHealth;
    }



    /// <summary>
    /// 重写依赖属性方法，指定Health依赖于MaxHealth
    /// </summary>
    /// <returns>依赖的属性类型列表</returns>
    public override IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return [GameAttributes.MaxHealth];
    }
}
