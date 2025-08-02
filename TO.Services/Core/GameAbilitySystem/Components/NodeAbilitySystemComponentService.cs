using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Nodes.Abstractions.Core.GameAbilitySystem;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Services.Bases;

namespace TO.Services.Core.GameAbilitySystem.Components;

/// <summary>
/// Ability System Component 服务实现
/// 提供ASC的核心业务逻辑
/// </summary>
public class NodeAbilitySystemComponentService : BaseService, INodeAbilitySystemComponentService
{
    /// <summary>
    /// 属性管理器服务
    /// </summary>
    private readonly IAttributeManagerService _attributeManagerService;
    
    private readonly IAbilitySystemComponent _abilitySystemComponent;

    private readonly IAttributeDatabaseReadService _attributeDatabaseReadService;
    private readonly IGameplayEffectDatabaseReadService _gameplayEffectDatabaseReadService;
    
    private Guid _currentAttributeSetId;
    
    /// <summary>
    /// Ability System Component 服务实现
    /// 提供ASC的核心业务逻辑
    /// </summary>
    public NodeAbilitySystemComponentService(IAttributeManagerService attributeManagerService,
        IAbilitySystemComponent abilitySystemComponent, 
        IAttributeDatabaseReadService attributeDatabaseReadService, 
        IGameplayEffectDatabaseReadService gameplayEffectDatabaseReadService)
    {
        _attributeManagerService = attributeManagerService;
        _abilitySystemComponent = abilitySystemComponent;
        _attributeDatabaseReadService = attributeDatabaseReadService;
        _gameplayEffectDatabaseReadService = gameplayEffectDatabaseReadService;


        var attributeSets = _attributeDatabaseReadService.GetAttributeSetById(_abilitySystemComponent.AttributeSetId);
        _attributeManagerService.RegisterAttributeSet(attributeSets);
        _currentAttributeSetId = attributeSets.Id;
        _abilitySystemComponent.OnGetAttributeSetId += OnGetAttributeSetId;
        _abilitySystemComponent.OnGetAttributeValue += OnGetAttributeValue;
        _abilitySystemComponent.OnApplyEffect += OnApplyEffect;
    }

    private void OnGetAttributeSetId(Action<Guid> callback)
    {
        callback(_currentAttributeSetId);
    }
    
    private void OnGetAttributeValue(AttributeDefinition attributeType,Action<float> callback)
    {
        callback(_attributeManagerService.GetAttributeValue(_currentAttributeSetId, attributeType)!.CurrentValue);
    }
    
    private void OnApplyEffect(string effectId)
    {
        var effect = _gameplayEffectDatabaseReadService.GetEffectByAttributeSetId(effectId);
        _attributeManagerService.ApplyEffect(_currentAttributeSetId, effect);
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _abilitySystemComponent.OnGetAttributeSetId -= OnGetAttributeSetId;
        _abilitySystemComponent.OnApplyEffect -= OnApplyEffect;
    }
}
