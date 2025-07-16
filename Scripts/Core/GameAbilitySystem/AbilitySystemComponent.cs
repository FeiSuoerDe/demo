using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Godot;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Nodes.Abstractions.Core.GameAbilitySystem;
using TO.Services.Abstractions.Core.GameAbilitySystem;
using TO.Services.Core.GameAbilitySystem;

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
    
    public override void _Ready()
    {
        base._Ready();
        // 注册到依赖注入容器
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<IAbilitySystemComponent, AbilitySystemComponentService>(this);
       
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }

    
   
    
}