using TO.Commons.Enums.Game;
using TO.Data.Database;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Repositories.Abstractions.Core.ReadTableSystem;

namespace TO.Services.Core.ReadTableSystem
{
    public class GameplayEffectDatabaseReadService(IAttributeEffectCacheRepo cache) : IGameplayEffectDatabaseReadService
    {
        public AttributeEffect GetEffectByAttributeSetId(string id)
        {
            var cachedEffect = cache.GetEffect(id);
            if (cachedEffect != null)
            {
                return cachedEffect.Clone();
            }

            using var context = new GameplayEffectDatabaseContext();

            var effectEntity = context.AttributeEffects.FirstOrDefault(e => e.Id == id);
            if (effectEntity == null) return null!;

            var modelModifiers = context.AttributeModifiers.Where(m => m.EffectId == id).ToList()
                .Select(m => new AttributeModifier(m.AttributeType, m.OperationType, m.Value, m.ExecutionOrder))
                .ToList();

            var effect = new AttributeEffect(effectEntity.Name, effectEntity.Description, modelModifiers,
                new Duration(effectEntity.IsInfinite, effectEntity.DurationSeconds), effectEntity.EffectType,
                 effectEntity.Tags,
                 effectEntity.StackingType, effectEntity.MaxStacks, effectEntity.Priority,
                effectEntity.IsPassive, effectEntity.IsPeriodic, effectEntity.IntervalSeconds);



            cache.CacheEffect(id, effect);
            return effect;
        }
    }
}
