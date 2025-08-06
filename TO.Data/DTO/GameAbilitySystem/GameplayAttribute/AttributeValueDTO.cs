using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TO.Data.DTO.GameAbilitySystem.GameplayAttribute;

public class AttributeValueDTO
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string AttributeSetId { get; set; }
    // Removing the navigation property back to AttributeSet to break a potential circular reference,
    // which might be causing issues with how EF Core materializes the object graph.
    // The relationship is still fully defined by the foreign key property above.
    // public AttributeSetDTO AttributeSet { get; set; }

    [Required]
    public string AttributeType { get; set; } // This is the foreign key, and it's TEXT in the database.
    [ForeignKey(nameof(AttributeType))]
    public AttributeDefinitionDTO AttributeDefinition { get; set; }
    
    public float BaseValue { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}
