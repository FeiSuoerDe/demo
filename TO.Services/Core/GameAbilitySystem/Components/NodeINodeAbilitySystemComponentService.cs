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
public class NodeINodeAbilitySystemComponentService : BaseService, INodeAbilitySystemComponentService
{
    /// <summary>
    /// 属性管理器服务
    /// </summary>
    private readonly IAttributeManagerService _attributeManagerService;
    
    private readonly IAbilitySystemComponent _abilitySystemComponent;

    private readonly IAttributeDatabaseReadService _attributeDatabaseReadService;
    
    /// <summary>
    /// Ability System Component 服务实现
    /// 提供ASC的核心业务逻辑
    /// </summary>
    public NodeINodeAbilitySystemComponentService(IAttributeManagerService attributeManagerService,
        IAbilitySystemComponent abilitySystemComponent, 
        IAttributeDatabaseReadService attributeDatabaseReadService)
    {
        _attributeManagerService = attributeManagerService;
        _abilitySystemComponent = abilitySystemComponent;
        _attributeDatabaseReadService = attributeDatabaseReadService;
        

        var attributeSets = _attributeDatabaseReadService.GetAttributeSetById(_abilitySystemComponent.AttributeSetId);
        _attributeManagerService.RegisterAttributeSet(attributeSets);
        
    }

   
}