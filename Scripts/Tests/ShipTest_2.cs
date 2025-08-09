using Godot;
using TimelapseInvoices.Scripts.Core.GameAbilitySystem;
using TimelapseInvoices.Scripts.UI.HUD;

namespace TimelapseInvoices.Scripts.Tests;

public partial class ShipTest_2 : Sprite2D
{
    [Export]
    private Area2D _area2D;
    [Export]
    private AbilitySystemComponent _abilitySystemComponent;
    
    [Export]
    private TestBarHud _testBarHud;
   
    public override void _Ready()
    {
        _area2D.AreaEntered += OnAreaEntered;
        _testBarHud.Bind(_abilitySystemComponent.GetAttributeSetId());
        
    }
    
    private void OnAreaEntered(Node2D body)
    {
        GD.Print("Body entered: " + body.Name);
        if (body.GetParent() is not IBullet bullet) return;
        bullet.Hit(_abilitySystemComponent);
        bullet.Destroy();
    }
}