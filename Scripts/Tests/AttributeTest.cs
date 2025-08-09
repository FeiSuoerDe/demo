using System;
using Autofac;
using Godot;
using TimelapseInvoices.Scripts.Core.GameAbilitySystem;
using TimelapseInvoices.Scripts.UI.HUD;

namespace TimelapseInvoices.Scripts.Tests;

public partial class AttributeTest : Node
{
    [Export]
    public AbilitySystemComponent AbilitySystemComponent { get; set; }
    
    [Export]
    public TestBarHud TestBarHud { get; set; }
    
    [Export]
    private Button hurtButton { get; set; }
    [Export]
    private Button healButton { get; set; }
    
    [Export]
    private Button levelUpButton { get; set; }

    [Export]
    private Button subButton { get; set; }

    public Guid AttributeSetId { get; set; }
    public ILifetimeScope? NodeScope { get; set; }

    public override void _Ready()
    {
        AttributeSetId = AbilitySystemComponent.GetAttributeSetId();
        TestBarHud.Bind(AttributeSetId);

        hurtButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_damage",null);
        healButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_heal",null);
        levelUpButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_constitution_boost",null);
        subButton.Pressed += () => AbilitySystemComponent.ApplyEffect("d5ea8291-344b-4c23-aacc-a907a849b3c8",null);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
    


}