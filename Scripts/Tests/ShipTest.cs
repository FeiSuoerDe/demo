using Godot;
using System;
using demo.Core.GameAbilitySystem;
using demo.Tests;
using TO.Commons.Enums.Game;

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
            _abilitySystemComponent.GetAttributeValue(AttributeType.Speed, speed =>
            {
                Position += new Vector2(0, -1) * speed * Position * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.A))
        {
            _abilitySystemComponent.GetAttributeValue(AttributeType.Speed, speed =>
            {
                Position += new Vector2(-1, 0) * speed * Position * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.D))
        {
            _abilitySystemComponent.GetAttributeValue(AttributeType.Speed, speed =>
            {
                Position += new Vector2(1, 0) * speed * Position * (float)delta;
            });
        }
        if (Input.IsKeyPressed(Key.S))
        {
            _abilitySystemComponent.GetAttributeValue(AttributeType.Speed, speed =>
            {
                Position += new Vector2(0, 1) * speed * Position * (float)delta;
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
