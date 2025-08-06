using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Repositories.Abstractions.Core.ReadTableSystem;

namespace TO.Repositories.Core.ReadTableSystem;

public class AttributeSetCacheRepo : IAttributeSetCacheRepo
{
    private readonly Dictionary<string, AttributeSet> _cache = new Dictionary<string, AttributeSet>();

    public AttributeSet? GetAttributeSet(string id)
    {
        return _cache.TryGetValue(id, out var set) ? set : null;
    }

    public void CacheAttributeSet(string id, AttributeSet attributeSet)
    {
        _cache[id] = attributeSet;
    }
}