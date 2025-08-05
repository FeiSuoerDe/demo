using System;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Factories;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AttributeValueProviderAttribute(string attributeKey) : Attribute
{
    public string AttributeKey { get; } = attributeKey;
}
