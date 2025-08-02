using demo.Core.GameAbilitySystem;
using Godot;

namespace demo.Tests;

public partial class ShipTest_2 : Sprite2D
{
    [Export]
    private Area2D _area2D;
    [Export]
    private AbilitySystemComponent _abilitySystemComponent;
    
    [Export]
    private UI.HUD.TestBarHud _testBarHud;
   
    public override void _Ready()
    {
        _area2D.AreaEntered += OnAreaEntered;
        _abilitySystemComponent.GetAttributeSetId(guid => _testBarHud.Bind(guid));
        
    }
    
    private void OnAreaEntered(Node2D body)
    {
        GD.Print("Body entered: " + body.Name);
        if (body.GetParent() is not IBullet bullet) return;
        bullet.Hit(_abilitySystemComponent);
        bullet.Destroy();
    }
}