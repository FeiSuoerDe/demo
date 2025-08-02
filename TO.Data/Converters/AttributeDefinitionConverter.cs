using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Registries;

namespace TO.Data.Converters;

/// <summary>
/// Converts an AttributeDefinition to its string Key for database storage, and back.
/// </summary>
public class AttributeDefinitionConverter : ValueConverter<AttributeDefinition, string>
{
    public AttributeDefinitionConverter() : base(
        attribute => attribute.Key,
        key => AttributeRegistry.Get(key))
    {
    }
}