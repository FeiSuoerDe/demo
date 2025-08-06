using System;
using Autofac;
using Godot;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Nodes.Abstractions.Core.GameAbilitySystem;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility;
using TO.Services.Core.GameAbilitySystem.Components;

namespace demo.Core.GameAbilitySystem;

/// <summary>
/// Ability System Component 实现类
/// 管理游戏能力（Abilities）、属性（Attributes）、效果（Effects）的核心组件
/// 任何需与GAS交互的Actor必须挂载ASC
/// </summary>
[GlobalClass]
public partial class AbilitySystemComponent : Node, IAbilitySystemComponent
{
    [Export]
    public string AttributeSetId{get; set;}
    
    /// <summary>
    /// 依赖注入容器作用域
    /// </summary>
    public ILifetimeScope? NodeScope { get; set; }
    
    // 【新增】用于简化通信和暴露清晰的API
    public INodeAbilitySystemComponentService Service { get; private set; }

    
    public event Action<Action<Guid>>? OnGetAttributeSetId;
    
    public event Action<AttributeDefinition,Action<float>>? OnGetAttributeValue;
    
    
    
    public override void _Ready()
    {
        base._Ready();
        // 注册到依赖注入容器
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<IAbilitySystemComponent, NodeAbilitySystemComponentService>(this);
        // 从该 Scope 中解析出服务实例，并持有引用以便直接调用
        Service = NodeScope.Resolve<INodeAbilitySystemComponentService>();

    }
    
    public Guid GetAttributeSetId()
    {
        return Service.CurrentAttributeSetId;
    }
    
    public float GetAttributeValue(AttributeDefinition attributeType)
    {
        return Service.OnGetAttributeValue(attributeType);
    }
    
    public void ApplyEffect(string effectId, GameplayEffectSource? effectSource)
    {
        Service.OnApplyEffect(effectId,effectSource);
    }
   
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }

    
   
    
}
