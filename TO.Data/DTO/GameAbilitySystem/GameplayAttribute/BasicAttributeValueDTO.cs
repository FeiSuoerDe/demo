using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.DTO.GameAbilitySystem.GameplayAttribute;

public class BasicAttributeValueDTO
{
    public int Id { get; set; }
    public required string AttributeSetId { get; set; }
    public AttributeDefinition AttributeType { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float BaseValue { get; set; }
}