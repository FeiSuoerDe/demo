using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;

public interface IAttributeManagerService
{
    void RegisterAttributeSet(AttributeSet attributeSet);
    void UnregisterAttributeSet(Guid attributeSetId);
    AttributeSet? GetAttributeSet(Guid attributeSetId);
    IEnumerable<AttributeSet> GetAllAttributeSets();
    bool ApplyEffect(Guid attributeSetId, AttributeEffect? effect);
    bool RemoveEffect(Guid attributeSetId, Guid effectId);
    
    AttributeValue? GetAttributeValue(Guid attributeSetId, AttributeDefinition attributeType);
 
    void UpdateEffectDurations(float deltaTime);
}