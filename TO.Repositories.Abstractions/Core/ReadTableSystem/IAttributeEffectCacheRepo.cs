using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Repositories.Abstractions.Core.ReadTableSystem
{
    public interface IAttributeEffectCacheRepo
    {
        AttributeEffect? GetEffect(string attributeSetId);
        void CacheEffect(string attributeSetId, AttributeEffect effects);
    }
}