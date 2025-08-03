using System;
using Autofac;
using demo.Core.GameAbilitySystem;
using Godot;

namespace demo.Tests;

public partial class AttributeTest : Node
{
    [Export]
    public AbilitySystemComponent AbilitySystemComponent { get; set; }
    
    [Export]
    public UI.HUD.TestBarHud TestBarHud { get; set; }
    
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
        AbilitySystemComponent.GetAttributeSetId(guid => AttributeSetId = guid);
        TestBarHud.Bind(AttributeSetId);

        hurtButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_damage");
        healButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_heal");
        levelUpButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_constitution_boost");
        subButton.Pressed += () => AbilitySystemComponent.ApplyEffect("d5ea8291-344b-4c23-aacc-a907a849b3c8");
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
    


}