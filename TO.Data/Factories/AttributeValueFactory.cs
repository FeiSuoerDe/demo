using TO.Data.Attributes;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute.CharacterAttributes;

namespace TO.Data.Factories;

/// <summary>
/// 属性值工厂，用于创建不同类型的属性值实例
/// </summary>
public static class AttributeValueFactory
{
    /// <summary>
    /// 根据属性类型创建属性值实例
    /// </summary>
    /// <param name="attributeType">属性定义</param>
    /// <param name="baseValue">基础值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>创建的属性值实例</returns>
    public static AttributeValue Create(AttributeDefinition attributeType, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        if (attributeType == GameAttributes.Health)
        {
            return new HealthAttributeValue(attributeType, baseValue, minValue, maxValue);
        }
        if (attributeType == GameAttributes.MaxHealth)
        {
            return new MaxHealthAttributeValue(attributeType, baseValue, minValue, maxValue);
        }
        // 在这里可以为其他属性类型添加更多的if语句
        return new AttributeValue(attributeType, baseValue, minValue, maxValue);
    }
}