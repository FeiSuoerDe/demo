using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Factories;

/// <summary>
/// 属性值工厂，用于创建不同类型的属性值实例
/// </summary>
public static class AttributeValueFactory
{
    private static readonly Dictionary<string, Type> _attributeValueProviders = new();

    static AttributeValueFactory()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化工厂，扫描程序集以查找属性值提供程序
    /// </summary>
    public static void Initialize()
    {
        _attributeValueProviders.Clear();
        var types = typeof(AttributeValueFactory).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AttributeValue)));

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<AttributeValueProviderAttribute>();
            if (attribute != null)
            {
                _attributeValueProviders[attribute.AttributeKey] = type;
                GD.Print(attribute.AttributeKey);
            }
        }
    }

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
        if (_attributeValueProviders.TryGetValue(attributeType.Key, out var providerType))
        {
            var instance = Activator.CreateInstance(providerType, attributeType, baseValue, minValue, maxValue);
            return instance as AttributeValue ?? throw new InvalidOperationException($"Failed to create or cast instance of type {providerType.FullName} for attribute key {attributeType.Key}.");
        }
        
        return new AttributeValue(attributeType, baseValue, minValue, maxValue);
    }
}
