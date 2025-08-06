using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Attributes;

public static partial class GameAttributes
{
    // Character Attributes
    public static readonly AttributeDefinition Health = new(0, HealthKey, "Character");
    public static readonly AttributeDefinition MaxHealth = new(1, MaxHealthKey, "Character");
    public static readonly AttributeDefinition Energy = new(2, "Character.Energy", "Character");
    public static readonly AttributeDefinition Speed = new(3, "Character.Speed", "Character");
        
    // Character Core Attributes
    public static readonly AttributeDefinition Intelligence = new(4, "Character.Core.Intelligence", "Character.Core");
    public static readonly AttributeDefinition Perception = new(5, "Character.Core.Perception", "Character.Core");
    public static readonly AttributeDefinition Charisma = new(6, "Character.Core.Charisma", "Character.Core");
    public static readonly AttributeDefinition Will = new(7, "Character.Core.Will", "Character.Core");
    public static readonly AttributeDefinition Constitution = new(8, "Character.Core.Constitution", "Character.Core");
    public static readonly AttributeDefinition Agility = new(9, "Character.Core.Agility", "Character.Core");

    // Character Derived Attributes
    public static readonly AttributeDefinition LifeValue = new(10, "Character.Derived.LifeValue", "Character.Derived");
    public static readonly AttributeDefinition MentalValue = new(11, "Character.Derived.MentalValue", "Character.Derived");
    public static readonly AttributeDefinition MovementSpeed = new(12, "Character.Derived.MovementSpeed", "Character.Derived");
}