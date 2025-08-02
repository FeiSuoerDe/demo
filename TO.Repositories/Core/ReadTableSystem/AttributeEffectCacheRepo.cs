using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Repositories.Abstractions.Core.ReadTableSystem;

namespace TO.Repositories.Core.ReadTableSystem;

public class AttributeEffectCacheRepo : IAttributeEffectCacheRepo
{
    private readonly Dictionary<string, AttributeEffect> _cache = new();

    public AttributeEffect? GetEffect(string attributeSetId)
    {
        return _cache.TryGetValue(attributeSetId, out var effects) ? effects : null;
    }

    public void CacheEffect(string attributeSetId, AttributeEffect effects)
    {
        _cache[attributeSetId] = effects;
    }
}