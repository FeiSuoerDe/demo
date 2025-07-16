using System;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Enities.GameAbilitySystem.GameplayEffect
{
    public class AttributeModifierEntity
    {
        public int Id { get; set; }
        public string EffectId { get; set; }
        public AttributeType AttributeType { get; set; }
        public ModifierOperationType OperationType { get; set; }
        public float Value { get; set; }
        public int ExecutionOrder { get; set; }
        public SourceType SourceType { get; set; }
    }
}