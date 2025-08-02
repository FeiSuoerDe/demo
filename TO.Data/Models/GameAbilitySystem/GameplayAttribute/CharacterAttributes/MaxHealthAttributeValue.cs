using TO.Data.Attributes;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute.CharacterAttributes;

public class MaxHealthAttributeValue(AttributeDefinition attributeType, float baseValue, float minValue, float maxValue)
    : AttributeValue(attributeType, baseValue, minValue, maxValue)
{

    private const float Strength = 100f;
    
    public override float CustomCompute(IEnumerable<AttributeValue?> attributes)
    {
        var constitution = attributes.FirstOrDefault(a => a?.AttributeType == GameAttributes.Constitution);
        var constitutionValue = constitution?.CurrentValue ?? 0f;
        return BaseValue + constitutionValue * Strength;
    }

    /// <summary>
    /// 重写依赖属性方法，指定MaxHealth依赖于Constitution
    /// </summary>
    /// <returns>依赖的属性类型列表</returns>
    public override IEnumerable<AttributeDefinition> GetDependentAttributes()
    {
        return [GameAttributes.Constitution];
    }
}
