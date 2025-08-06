using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility;

/// <summary>
/// Ability System Component 服务接口
/// 提供ASC的业务逻辑处理
/// </summary>
public interface INodeAbilitySystemComponentService
{
    Guid CurrentAttributeSetId { get; }
    void OnApplyEffect(string effectId, GameplayEffectSource? effectSource);

    float OnGetAttributeValue(AttributeDefinition attributeType);
}