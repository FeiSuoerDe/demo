using System;
using System.Collections.Generic;
using TO.Commons.Enums.Game;

namespace TO.Data.Enities.GameAbilitySystem.GameplayEffect
{
    public class AttributeEffectEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EffectType EffectType { get; set; }
        public EffectStackingType StackingType { get; set; }
        public HashSet<EffectTags> Tags { get; set; }
        public float DurationSeconds { get; set; }
        public bool IsInfinite { get; set; }
        public int MaxStacks { get; set; }
        public bool IsPassive { get; set; }
        public int Priority { get; set; }

    }
}