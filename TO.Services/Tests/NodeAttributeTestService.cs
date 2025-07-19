using Godot;
using TO.Nodes.Abstractions.Tests;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Abstractions.Core.ReadTableSystem;
using TO.Services.Bases;

namespace TO.Services.Tests;

public class NodeAttributeTestService : BaseService
{
    private readonly IAttributeManagerService _attributeManagerService;
    private readonly IGameplayEffectDatabaseReadService _gameplayEffectDatabaseReadService;
    private readonly IAttributeTest _attributeTest;
    
    public NodeAttributeTestService(IAttributeManagerService attributeManagerService, IGameplayEffectDatabaseReadService gameplayEffectDatabaseReadService, IAttributeTest attributeTest)
    {
        _attributeManagerService = attributeManagerService;
        _gameplayEffectDatabaseReadService = gameplayEffectDatabaseReadService;
        _attributeTest = attributeTest;
        
        _attributeTest.OnHurt += OnHurt;
        _attributeTest.OnHeal += OnHeal;
    }

    private void OnHeal()
    {
        var effect = _gameplayEffectDatabaseReadService.GetEffectByAttributeSetId("effect_heal");
        _attributeManagerService.ApplyEffect(_attributeTest.AttributeSetId, effect);
    }

    private void OnHurt()
    {
        var effect = _gameplayEffectDatabaseReadService.GetEffectByAttributeSetId("effect_damage");
        _attributeManagerService.ApplyEffect(_attributeTest.AttributeSetId, effect);
    }
}