using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Attributes;

public static partial class GameAttributes
{
    // 飞船核心属性
    
    /// <summary>
    /// 辐能值
    /// </summary>
    public static readonly AttributeDefinition Flux = new(13, "Ship.Core.Flux", "Ship.Core");
    
    /// <summary>
    /// 最大辐能值
    /// </summary>
    public static readonly AttributeDefinition MaxFlux = new(14, "Ship.Core.MaxFlux", "Ship.Core");
    
    /// <summary>
    /// 护甲值
    /// </summary>
    public static readonly AttributeDefinition Armor = new(15, "Ship.Core.Armor", "Ship.Core");
    
    /// <summary>
    /// 最大护甲值
    /// </summary>
    public static readonly AttributeDefinition MaxArmor = new(16, "Ship.Core.MaxArmor", "Ship.Core");
    
    /// <summary>
    /// 船身结构值
    /// </summary>
    public static readonly AttributeDefinition Hull = new(17, HullKey, "Ship.Core");
    
    /// <summary>
    /// 最大船身结构值
    /// </summary>
    public static readonly AttributeDefinition MaxHull = new(18, MaxHullKey, "Ship.Core");
    
    /// <summary>
    /// 推进力
    /// </summary>
    public static readonly AttributeDefinition Thrust = new(19, "Ship.Core.Thrust", "Ship.Core");

    // 飞船武器属性
    
    /// <summary>
    /// 武器伤害值
    /// </summary>
    public static readonly AttributeDefinition WeaponDamage = new(20, "Ship.Weapon.Damage", "Ship.Weapon");
    
    /// <summary>
    /// 对盾伤害倍率值
    /// </summary>
    public static readonly AttributeDefinition DamageMultiplierVsShields = new(21, "Ship.Weapon.DamageMultiplierVsShields", "Ship.Weapon");
    
    /// <summary>
    /// 护盾伤害减免值
    /// </summary>
    public static readonly AttributeDefinition ShieldDamageReduction = new(22, "Ship.Weapon.ShieldDamageReduction", "Ship.Weapon");
    
    /// <summary>
    /// 对甲倍率值
    /// </summary>
    public static readonly AttributeDefinition DamageMultiplierVsArmor = new(23, "Ship.Weapon.DamageMultiplierVsArmor", "Ship.Weapon");
    
    /// <summary>
    /// 对船体倍率值
    /// </summary>
    public static readonly AttributeDefinition DamageMultiplierVsHull = new(24, DamageMultiplierVsHullKey, "Ship.Weapon");
}
