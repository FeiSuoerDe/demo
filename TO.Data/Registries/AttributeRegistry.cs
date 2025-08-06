using System.Reflection;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Registries;

/// <summary>
/// A central registry for all AttributeDefinitions.
/// It discovers and stores all defined attributes from the assembly.
/// </summary>
public static class AttributeRegistry
{
    private static readonly Dictionary<string, AttributeDefinition> _attributesByKey = new();
    private static readonly Dictionary<int, AttributeDefinition> _attributesById = new();

    static AttributeRegistry()
    {
        // Discover all attributes defined in static partial classes like GameAttributes
        var attributeContainerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t.Name == "GameAttributes");

        foreach (var type in attributeContainerTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(AttributeDefinition));

            foreach (var field in fields)
            {
                var attribute = (AttributeDefinition)field.GetValue(null);
                if (attribute != null)
                {
                    if (!_attributesByKey.ContainsKey(attribute.Key))
                    {
                        _attributesByKey.Add(attribute.Key, attribute);
                        _attributesById.Add(attribute.Id, attribute);
                    }
                    else
                    {
                        // Handle potential duplicate key error
                        throw new InvalidOperationException($"Duplicate attribute key '{attribute.Key}' detected.");
                    }
                }
            }
        }
    }

    public static AttributeDefinition Get(string key) => _attributesByKey.GetValueOrDefault(key);
    public static AttributeDefinition Get(int id) => _attributesById.GetValueOrDefault(id);
    public static IReadOnlyCollection<AttributeDefinition> GetAll() => _attributesByKey.Values;
}