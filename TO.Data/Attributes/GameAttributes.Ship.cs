using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Attributes;

public static partial class GameAttributes
{
    // 飞船核心属性
    
    /// <summary>
    /// 表示飞船的引擎推力。
    /// </summary>
    public static readonly AttributeDefinition Thrust = new(13, "Ship.Core.Thrust", "Ship.Core");
    /// <summary>
    /// 表示飞船的护盾强度。
    /// </summary>
    public static readonly AttributeDefinition Shield = new(14, "Ship.Core.Shield", "Ship.Core");
    /// <summary>
    /// 表示飞船的装甲强度。
    /// </summary>
    public static readonly AttributeDefinition Armor = new(15, "Ship.Core.Armor", "Ship.Core");
    /// <summary>
    /// 表示飞船转向和改变方向的能力。
    /// </summary>
    public static readonly AttributeDefinition Maneuverability = new(16, "Ship.Core.Maneuverability", "Ship.Core");
    
    
}
