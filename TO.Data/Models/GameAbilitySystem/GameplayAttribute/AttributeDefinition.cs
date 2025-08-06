namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute;

/// <summary>
/// Represents the definition of a gameplay attribute.
/// This class replaces the large AttributeType enum for better scalability and maintainability.
/// </summary>
public sealed class AttributeDefinition
{
    /// <summary>
    /// A unique identifier for the attribute. For legacy attributes, this can match the old enum integer value.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The unique string-based key for the attribute (e.g., "Character.Core.Health").
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// A category for grouping attributes, e.g., "Character.Core", "Ship.Specs".
    /// </summary>
    public string Category { get; }

    public AttributeDefinition(int id, string key, string category)
    {
        Id = id;
        Key = key;
        Category = category;
    }

    public override string ToString() => Key;
        
}