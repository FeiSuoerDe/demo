namespace TO.Data.Attributes;

/// <summary>
/// Container for all gameplay attribute definitions.
/// This is a partial class, allowing definitions to be spread across multiple files.
/// </summary>
public static partial class GameAttributes
{
    public const string HealthKey = "Character.Health";
    public const string MaxHealthKey = "Character.MaxHealth";
    
    public const string HullKey = "Ship.Core.Hull";
    public const string MaxHullKey = "Ship.Core.MaxHull";
    public const string ArmorKey = "Ship.Core.Armor";
    public const string WeaponsKey = "Ship.Core.Weapons";
    
    public const string DamageMultiplierVsHullKey = "Ship.Weapon.DamageMultiplierVsHull";
}
