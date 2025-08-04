using System;
using Autofac;
using Godot;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Nodes.Abstractions.Core.GameAbilitySystem;
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
    
    public event Action<Action<Guid>>? OnGetAttributeSetId;
    
    public event Action<AttributeDefinition,Action<float>>? OnGetAttributeValue;
    

    public event Action<string, GameplayEffectSource?>? OnApplyEffect; 
    
    public override void _Ready()
    {
        base._Ready();
        // 注册到依赖注入容器
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<IAbilitySystemComponent, NodeAbilitySystemComponentService>(this);
    }
    
    public void GetAttributeSetId(Action<Guid> callback)
    {
        OnGetAttributeSetId?.Invoke(callback);
    }
    
    public void GetAttributeValue(AttributeDefinition attributeType,Action<float> callback)
    {
        OnGetAttributeValue?.Invoke(attributeType,callback);
    }
    
    public void ApplyEffect(string effectId, GameplayEffectSource? effectSource)
    {
        OnApplyEffect?.Invoke(effectId,effectSource);
    }
   
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }

    
   
    
}
