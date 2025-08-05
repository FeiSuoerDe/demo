using Godot;
using TO.Data.Attributes;
using TO.Data.Factories;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute.ShipAttributes;

[AttributeValueProvider(GameAttributes.HullKey)]
public class HullAttributeValue(
    AttributeDefinition attributeType,
    float baseValue,
    float minValue,
    float maxValue)
    : AttributeValue(attributeType, baseValue, minValue, maxValue)
{
    private float? _previousMaxHull;

    public override float CustomCompute(IEnumerable<AttributeValue?> attributes)
    {
        var maxHullAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxHull);
        if (maxHullAttr == null)
        {
            return CurrentValue;
        }

        var currentMaxHull = maxHullAttr.CurrentValue;
        _previousMaxHull ??= currentMaxHull;
        
        // 如果最大船体值发生变化，按比例调整当前船体值
        if (Math.Abs(_previousMaxHull.Value - currentMaxHull) > float.Epsilon)
        {
            var percentage = _previousMaxHull.Value == 0 ? 1.0f : CurrentValue / _previousMaxHull.Value;
            return percentage * currentMaxHull;
        }

        return CurrentValue;
    }
    
    public override void ComputeValue(IEnumerable<AttributeValue?> attributes, AttributeModifier? modifier,Dictionary<AttributeDefinition, float>? effectSource = null)
    {
        var maxHullAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxHull);
        var currentMaxHull = maxHullAttr?.CurrentValue ?? CurrentValue;

        if (effectSource != null)
        {
            var damage = effectSource[GameAttributes.WeaponDamage];
            var damageMultiplierVsHull = effectSource[GameAttributes.DamageMultiplierVsHull];
            
            var finalDamage = damage * damageMultiplierVsHull;
            
            modifier?.SetModifierValue(finalDamage);
        }
        // 1. 通过CustomCompute获取基于依赖计算的当前值
        var computedValue = CustomCompute(attributes);
        
        // 2. 应用修改器
        var modifiedValue = modifier?.ExecuteModifier(computedValue) ?? computedValue;

        // 3. 确保值在有效范围内并转换为整数
        SetCurrentValue((int)Math.Clamp(modifiedValue, 0, currentMaxHull));
        
        // 4. 更新上一次的最大生命值以备下次计算
        _previousMaxHull = currentMaxHull;
    }
    
    public override IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return [GameAttributes.MaxHull];
    }
    
}
