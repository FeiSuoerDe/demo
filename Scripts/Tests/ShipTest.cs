using demo.Core.GameAbilitySystem;
using Godot;
using TO.Data.Attributes;

namespace demo.Tests;

public partial class ShipTest : Sprite2D
{
    [Export]
    private AbilitySystemComponent _abilitySystemComponent;
    
    [Export]
    private PackedScene _bulletPrefab;

    [Export] private Node _scene;
   

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.W))
        {
            _abilitySystemComponent.GetAttributeValue(GameAttributes.Speed, speed =>
            {
                Position += new Vector2(0, -1) * speed * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.A))
        {
            _abilitySystemComponent.GetAttributeValue(GameAttributes.Speed, speed =>
            {
                Position += new Vector2(-1, 0) * speed * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.D))
        {
            _abilitySystemComponent.GetAttributeValue(GameAttributes.Speed, speed =>
            {
                Position += new Vector2(1, 0) * speed * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.S))
        {
            _abilitySystemComponent.GetAttributeValue(GameAttributes.Speed, speed =>
            {
                Position += new Vector2(0, 1) * speed * (float)delta;
            });
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton is { Pressed: true })
            {
                var bullet = _bulletPrefab.Instantiate<Bullet>();
                bullet.Position = Position;
                _scene.AddChild(bullet);
                
                bullet.Init((mouseButton.GlobalPosition - Position).Normalized());
                GD.Print("Bullet fired!");
            }
        }
    }

}
