using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TO.Data.DTO.GameAbilitySystem.GameplayAttribute;

[Table("Attributes")] // Explicitly map this DTO to the "Attributes" table in the database.
public class AttributeDefinitionDTO
{
    [Key]
    public string Id { get; set; } // The primary key in the database is TEXT, so this must be a string.
    public string Category { get; set; }
    public string Description { get; set; } // The "Description" column exists in the database.
}
