
using System.Text;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;


namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// 属性集
/// </summary>
public class AttributeSet
{
    /// <summary>
    /// 属性集唯一标识
    /// </summary>
    public Guid Id { get; }
        
    /// <summary>
    /// 属性列表 - 优化后移除冗余的AttributeType键
    /// </summary>
    public List<AttributeValue> Attributes { get; }

    private Dictionary<AttributeDefinition, List<AttributeValue>> DerivedAttributes { get; }
        
    /// <summary>
    /// 属性变化事件
    /// </summary>
    public event Action<AttributeDefinition, float, float>? AttributeChanged;
        
    public event Action<AttributeDefinition, float, float>? AttributeRangeChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="attributes"></param>
    public AttributeSet(List<AttributeValue> attributes)
    {
        Id = Guid.NewGuid();
        Attributes = attributes;
        DerivedAttributes = new Dictionary<AttributeDefinition, List<AttributeValue>>();
        InitializeDerivedAttributes();
        InitializeAttributeSet();
    }


    /// <summary>
    /// 获取属性值
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <returns>属性值，如果不存在则返回null</returns>
    public AttributeValue? GetAttribute(AttributeDefinition type)
    {
        return Attributes.FirstOrDefault(attr => attr.AttributeType == type);
    }
        
    /// <summary>
    /// 检查是否拥有指定属性
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <returns>是否拥有该属性</returns>
    public bool HasAttribute(AttributeDefinition type)
    {
        return Attributes.Any(attr => attr.AttributeType == type);
    }
        
    /// <summary>
    /// 获取所有属性类型
    /// </summary>
    /// <returns>属性类型集合</returns>
    public IEnumerable<AttributeDefinition> GetAllAttributeTypes()
    {
        return Attributes.Select(attr => attr.AttributeType);
    }
        
    /// <summary>
    /// 获取所有属性值
    /// </summary>
    /// <returns>属性值集合</returns>
    public IEnumerable<AttributeValue?> GetAllAttributeValues()
    {
        return Attributes;
    }
        
    /// <summary>
    /// 获取所有属性值
    /// </summary>
    /// <returns>属性值集合</returns>
    public IEnumerable<AttributeValue?> GetAllAttributeValues(IEnumerable<AttributeDefinition> types)
    {
        return Attributes.Where(attr => types.Contains(attr.AttributeType));
    }
        
    public void SetAttributeCurrentValue(AttributeDefinition type, AttributeModifier? modifier, GameplayEffectSource? effectSource)
    {
        var oldValue = GetAttributeCurrentValue(type);
            
        var attribute = GetAttribute(type);
        if (attribute != null)
        {
            attribute.ComputeValue(GetAllAttributeValues(attribute.GetDependentAttributes()), modifier,effectSource?.Attributes);
        }
        else
        {
            return;
        }

        if (DerivedAttributes.TryGetValue(type, out var derivedList))
        {
            foreach (var derivedAttr in derivedList)
            {
                SetAttributeCurrentValue(derivedAttr.AttributeType, null,effectSource);
            }
        }
            
        var newValue = GetAttributeCurrentValue(type);
        OnAttributeChanged(type, oldValue, newValue);
    }
        
    /// <summary>
    /// 设置属性基础值
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <param name="value">基础值</param>
    public void SetAttributeBaseValue(AttributeDefinition type, float value)
    {
        var oldValue = GetAttributeCurrentValue(type);
            
        var attribute = GetAttribute(type);
        if (attribute != null)
        {
            attribute.SetBaseValue(value);
        }
        else
        {
            Attributes.Add(new AttributeValue(type, value));
        }
            
        var newValue = GetAttributeBaseValue(type);
        OnAttributeChanged(type, oldValue, newValue);
    }
        
        
        
    /// <summary>
    /// 设置属性值范围
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    public void SetAttributeMinValue(AttributeDefinition type, float minValue)
    {
        var attribute = GetAttribute(type);
        if (attribute == null) return;
        attribute.SetMinValue(minValue);
        OnAttributeRangeChanged(type, minValue, attribute.MaxValue);
    }
        
    public void SetAttributeMaxValue(AttributeDefinition type,float maxValue)
    {
        var attribute = GetAttribute(type);
        if (attribute == null) return;
        attribute.SetMaxValue(maxValue);
        OnAttributeRangeChanged(type, attribute.MinValue, maxValue);
    }
        
    /// <summary>
    /// 获取属性当前值
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <returns>当前值</returns>
    public float GetAttributeCurrentValue(AttributeDefinition type)
    {
        var attribute = GetAttribute(type);
        return attribute?.CurrentValue ?? 0f;
    }
        
    /// <summary>
    /// 获取属性基础值
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <returns>基础值</returns>
    public float GetAttributeBaseValue(AttributeDefinition type)
    {
        var attribute = GetAttribute(type);
        return attribute?.BaseValue ?? 0f;
    }
        
    /// <summary>
    /// 初始化属性
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <param name="baseValue">基础值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    protected void InitializeAttribute(AttributeDefinition type, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        var attribute = GetAttribute(type);
        if (attribute == null)
        {
            Attributes.Add(new AttributeValue(type, baseValue, minValue, maxValue));
        }
        else
        {
            attribute.SetMaxValue(maxValue);
            attribute.SetMinValue(minValue);
            attribute.SetBaseValue(baseValue);
        }
    }
        
        
    /// <summary>
    /// 属性变化时的回调
    /// </summary>
    /// <param name="attributeType">属性类型</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    private void OnAttributeChanged(AttributeDefinition attributeType, float oldValue, float newValue)
    {
        AttributeChanged?.Invoke(attributeType, oldValue, newValue);
    }
        
    private void OnAttributeRangeChanged(AttributeDefinition attributeType, float minValue, float maxValue)
    {
        AttributeRangeChanged?.Invoke(attributeType, minValue, maxValue);
    }
        
    private void InitializeDerivedAttributes()
    {
        foreach (var attribute in Attributes)
        {
            var dependencies = attribute.GetDependentAttributes();

            foreach (var dependencyType in dependencies)
            {
                if (!DerivedAttributes.ContainsKey(dependencyType))
                {
                    DerivedAttributes[dependencyType] = [];
                }
                    
                if (!DerivedAttributes[dependencyType].Contains(attribute))
                {
                    DerivedAttributes[dependencyType].Add(attribute);
                }
            }
        }
    }

    /// <summary>
    /// 初始化属性集，计算所有依赖属性的值
    /// </summary>
    private void InitializeAttributeSet()
    {
        // 遍历所有属性，计算依赖属性的值
        foreach (var attribute in Attributes)
        {
            // attribute.ComputeValue(GetAllAttributeValues(attribute.GetDependentAttributes()), null);
            SetAttributeCurrentValue(attribute.AttributeType, null,null);
        }
            
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"AttributeSet Id: {Id}");
        sb.AppendLine("Attributes:");
        foreach (var attr in Attributes)
        {
            sb.AppendLine($"  {attr.AttributeType.Key}: Base = {attr.BaseValue}, Current = {attr.CurrentValue}");
        }
        return sb.ToString();
    }
}