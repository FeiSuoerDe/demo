using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.DTO.GameAbilitySystem.GameplayEffect;

public class AttributeModifierDTO
{

    public int Id { get; set; }
    public string EffectId { get; set; }
    public AttributeDefinition AttributeType { get; set; }
    public ModifierOperationType OperationType { get; set; }
    public float Value { get; set; }
    public int ExecutionOrder { get; set; }
}
