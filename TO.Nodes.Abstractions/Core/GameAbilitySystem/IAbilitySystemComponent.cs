using System;
using System.Collections.Generic;
using Autofac;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.Core.GameAbilitySystem;

/// <summary>
/// Ability System Component 接口
/// 管理游戏能力（Abilities）、属性（Attributes）、效果（Effects）的核心组件
/// 任何需与GAS交互的Actor必须挂载ASC
/// </summary>
public interface IAbilitySystemComponent : INode
{
    
    string AttributeSetId{get; set;}

}