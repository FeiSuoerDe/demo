using TO.Nodes.Abstractions.Core.GameAbilitySystem;
using TO.Services.Abstractions.Core.GameAbilitySystem;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Bases;

namespace TO.Services.Core.GameAbilitySystem.GameplayAbility;

/// <summary>
/// Ability System Component 服务实现
/// 提供ASC的核心业务逻辑
/// </summary>
public class AbilitySystemComponentService : BaseService, IAbilitySystemComponentService
{
    /// <summary>
    /// 属性管理器服务
    /// </summary>
    private readonly IAttributeManagerService _attributeManagerService;
    
    /// <summary>
    /// 属性计算服务
    /// </summary>
    private readonly IAttributeCalculationService _attributeCalculationService;
    
    private readonly IAbilitySystemComponent _abilitySystemComponent;

    /// <summary>
    /// Ability System Component 服务实现
    /// 提供ASC的核心业务逻辑
    /// </summary>
    public AbilitySystemComponentService(IAttributeManagerService attributeManagerService,
        IAttributeCalculationService attributeCalculationService,
        IAbilitySystemComponent abilitySystemComponent)
    {
        _attributeManagerService = attributeManagerService;
        _attributeCalculationService = attributeCalculationService;
        _abilitySystemComponent = abilitySystemComponent;
    }

   
}