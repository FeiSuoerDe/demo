using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;

public interface IAttributeManagerService
{
    void RegisterAttributeSet(AttributeSet attributeSet);
    void UnregisterAttributeSet(Guid attributeSetId);
    AttributeSet? GetAttributeSet(Guid attributeSetId);
    IEnumerable<AttributeSet> GetAllAttributeSets();
    bool ApplyEffect(Guid attributeSetId, Data.Models.GameAbilitySystem.GameplayEffect.AttributeEffect? effect);
    bool RemoveEffect(Guid attributeSetId, Guid effectId);
    
    AttributeValue? GetAttributeValue(Guid attributeSetId, AttributeType attributeType);
    bool SetAttributeValue(Guid attributeSetId, AttributeType attributeType, float value);
    void UpdateEffectDurations(float deltaTime);
}