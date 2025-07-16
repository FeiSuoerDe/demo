using System;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Enities.GameAbilitySystem.GameplayAttribute
{
    public class BasicAttributeValueEntity
    {
        public int Id { get; set; }
        public required string AttributeSetId { get; set; }
        public AttributeType AttributeType { get; set; }
        public float BaseValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
    }
}