using Godot;
using System;
using Autofac;
using demo.Core.GameAbilitySystem;


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

    public Guid AttributeSetId { get; set; }
    public ILifetimeScope? NodeScope { get; set; }
    
    public override void _Ready()
    {
        AbilitySystemComponent.GetAttributeSetId(guid => AttributeSetId = guid);
        TestBarHud.Bind(AttributeSetId);
        
        hurtButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_damage");
        healButton.Pressed += () => AbilitySystemComponent.ApplyEffect("effect_heal");
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
    


}
