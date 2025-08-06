using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.ReadTableSystem
{
    public interface IGameplayEffectDatabaseReadService
    {
        AttributeEffect GetEffectByAttributeSetId(string id);
    }
}