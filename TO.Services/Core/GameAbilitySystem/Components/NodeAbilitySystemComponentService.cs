using Godot;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
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

    public Guid CurrentAttributeSetId { get; }

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
        CurrentAttributeSetId = attributeSets.Id;
        
    }
    

    public float OnGetAttributeValue(AttributeDefinition attributeType)
    {
        return _attributeManagerService.GetAttributeValue(CurrentAttributeSetId, attributeType)!.CurrentValue;
    }
    
    public void OnApplyEffect(string effectId, GameplayEffectSource? effectSource)
    {
        var effect = _gameplayEffectDatabaseReadService.GetEffectByAttributeSetId(effectId);
        effect.SetSource(effectSource);
        _attributeManagerService.ApplyEffect(CurrentAttributeSetId, effect);
        GD.Print("Effect has been applied");
    }


}
