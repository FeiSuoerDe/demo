using Godot;
using System;
using Autofac;
using demo.Core.GameAbilitySystem;
using TO.Nodes.Abstractions.Tests;
using TO.Services.Tests;

public partial class AttributeTest : Node, IAttributeTest
{
    [Export]
    public AbilitySystemComponent AbilitySystemComponent { get; set; }
    
    [Export]
    public TestBarHud TestBarHud { get; set; }
    
    [Export]
    private Button hurtButton { get; set; }
    [Export]
    private Button healButton { get; set; }

    public Guid AttributeSetId { get; set; }
    public ILifetimeScope? NodeScope { get; set; }
    
    
    public event Action? OnHurt;
    public event Action? OnHeal;
    
    public override void _Ready()
    {
        AbilitySystemComponent.GetAttributeSetId(guid => AttributeSetId = guid);
        TestBarHud.Bind(AttributeSetId);
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<IAttributeTest, NodeAttributeTestService>(this);
        
        hurtButton.Pressed += () => OnHurt?.Invoke();
        healButton.Pressed += () => OnHeal?.Invoke();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
    


}
