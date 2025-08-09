using System.Collections.Generic;
using Godot;
using TimelapseInvoices.Scripts.Core.GameAbilitySystem;
using TO.Data.Attributes;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TimelapseInvoices.Scripts.Tests;

public partial class ShipTest : Sprite2D
{
    [Export]
    private AbilitySystemComponent _abilitySystemComponent;

    [Export]
    private AbilitySystemComponent _weaponAbilitySystemComponent;
    
    [Export]
    private PackedScene _bulletPrefab;

    [Export] private Node _scene;

    
    

    public override void _Process(double delta)
    {
        var speed = _abilitySystemComponent.GetAttributeValue(GameAttributes.Thrust);
        if (Input.IsKeyPressed(Key.W))
        {

            Position += new Vector2(0, -1) * speed * (float)delta;

        }

        if (Input.IsKeyPressed(Key.A))
        {

            Position += new Vector2(-1, 0) * speed * (float)delta;

        }

        if (Input.IsKeyPressed(Key.D))
        {

            Position += new Vector2(1, 0) * speed * (float)delta;
        }

        if (Input.IsKeyPressed(Key.S))
        {

            Position += new Vector2(0, 1) * speed * (float)delta;

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

                var value = _weaponAbilitySystemComponent.GetAttributeValue(GameAttributes.WeaponDamage);

                var dic = new Dictionary<AttributeDefinition, float>
                {
                    [GameAttributes.WeaponDamage] = value
                };
                bullet.Init((mouseButton.GlobalPosition - Position).Normalized(),dic);
                GD.Print("Bullet fired!");
            }
        }
    }

}
