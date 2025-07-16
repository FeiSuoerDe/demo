using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Repositories.Abstractions.Core.ReadTableSystem;

public interface IAttributeSetCacheRepo
{
    AttributeSet? GetAttributeSet(string id);
    void CacheAttributeSet(string id, AttributeSet attributeSet);
}