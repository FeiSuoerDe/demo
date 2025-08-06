using TO.Data.Attributes;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute.ShipAttributes;

public class FluxAttributeValue(
    AttributeDefinition attributeType,
    float baseValue,
    float minValue = Single.MinValue,
    float maxValue = Single.MaxValue)
    : AttributeValue(attributeType, baseValue, minValue, maxValue)
{
    private float? _previousMaxFlux;

    public override float CustomCompute(IEnumerable<AttributeValue?> attributes)
    {
        var maxHealthAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxFlux);
        if (maxHealthAttr == null)
        {
            return CurrentValue;
        }

        var currentMaxHealth = maxHealthAttr.CurrentValue;
        _previousMaxFlux ??= currentMaxHealth;
        
        // 如果最大辐能值发生变化，按比例调整当前生命值
        if (Math.Abs(_previousMaxFlux.Value - currentMaxHealth) > float.Epsilon)
        {
            var percentage = _previousMaxFlux.Value == 0 ? 1.0f : CurrentValue / _previousMaxFlux.Value;
            return percentage * currentMaxHealth;
        }

        return CurrentValue;
    }
    
    public override void ComputeValue(IEnumerable<AttributeValue?> attributes, AttributeModifier? modifier,Dictionary<AttributeDefinition, float>? effectSource = null)
    {
        var maxFluxAttr = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.MaxFlux);
        var currentMaxFlux = maxFluxAttr?.CurrentValue ?? CurrentValue;

        if (effectSource != null)
        {
            var damage = effectSource[GameAttributes.WeaponDamage];
            var damageMultiplierVsShields = effectSource[GameAttributes.DamageMultiplierVsShields];
            var shieldDamageReduction = effectSource[GameAttributes.ShieldDamageReduction];
            
            var finalDamage = damage * damageMultiplierVsShields * shieldDamageReduction;
            modifier?.SetModifierValue(finalDamage);
        }
        // 1. 通过CustomCompute获取基于依赖计算的当前值
        var computedValue = CustomCompute(attributes);
        
        // 2. 应用修改器
        var modifiedValue = modifier?.ExecuteModifier(computedValue) ?? computedValue;

        // 3. 确保值在有效范围内并转换为整数
        SetCurrentValue((int)Math.Clamp(modifiedValue, 0, currentMaxFlux));
        
        // 4. 更新上一次的最大生命值以备下次计算
        _previousMaxFlux = currentMaxFlux;
    }
    
    public override IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return [GameAttributes.MaxFlux];
    }
    
}