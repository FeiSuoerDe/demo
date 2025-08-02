using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Attributes;

public static partial class GameAttributes
{
    // Ship Core Attributes
    public static readonly AttributeDefinition Thrust = new(13, "Ship.Core.Thrust", "Ship.Core");
    public static readonly AttributeDefinition Shield = new(14, "Ship.Core.Shield", "Ship.Core");
    public static readonly AttributeDefinition Armor = new(15, "Ship.Core.Armor", "Ship.Core");
    public static readonly AttributeDefinition Maneuverability = new(16, "Ship.Core.Maneuverability", "Ship.Core");
    public static readonly AttributeDefinition Sensors = new(17, "Ship.Core.Sensors", "Ship.Core");

    // Ship Specification Attributes
    public static readonly AttributeDefinition Mass = new(18, "Ship.Specs.Mass", "Ship.Specs");
    public static readonly AttributeDefinition Length = new(19, "Ship.Specs.Length", "Ship.Specs");
    public static readonly AttributeDefinition Width = new(20, "Ship.Specs.Width", "Ship.Specs");
    public static readonly AttributeDefinition Height = new(21, "Ship.Specs.Height", "Ship.Specs");
    public static readonly AttributeDefinition CargoCapacity = new(22, "Ship.Specs.CargoCapacity", "Ship.Specs");
    public static readonly AttributeDefinition FuelCapacity = new(23, "Ship.Specs.FuelCapacity", "Ship.Specs");

    // Extended Attributes (Reserved)
    public static readonly AttributeDefinition CustomAttribute1 = new(24, "Custom.Attribute1", "Custom");
    public static readonly AttributeDefinition CustomAttribute2 = new(25, "Custom.Attribute2", "Custom");
    public static readonly AttributeDefinition CustomAttribute3 = new(26, "Custom.Attribute3", "Custom");
}