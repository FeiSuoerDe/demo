using System.Collections.Generic;
using Godot;
using TimelapseInvoices.Scripts.Core.GameAbilitySystem;
using TO.Commons.Enums.Game;
using TO.Data.Attributes;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TimelapseInvoices.Scripts.Tests;

public partial class Bullet : Node2D, IBullet
{
    [Export]
    public string _effectID;
    
    [Export]
    private Area2D _area;
    
    [Export]
    private AbilitySystemComponent _abilitySystemComponent;

    private Vector2 _direction;

    private Dictionary<AttributeDefinition, float> _attributes;

    private float lifeTime = 5f;
    public void Init(Vector2 direction,Dictionary<AttributeDefinition, float> attributes)
    {
        _direction = direction;
        _attributes = attributes;
    }
    
    public override void _Process(double delta)
    {
        Position += (float)delta * _direction * Position;
        lifeTime -= (float)delta;
        if (lifeTime <= 0)
        {
            QueueFree();
        }
    }
    
    public void Hit(AbilitySystemComponent asc)
    {
        var value = _abilitySystemComponent.GetAttributeValue(GameAttributes.DamageMultiplierVsHull);
        _attributes[GameAttributes.DamageMultiplierVsHull] = value;
        asc.ApplyEffect(_effectID,new GameplayEffectSource(SourceType.Equipment,this,Position,_attributes));
    }

    public void Destroy()
    {
        QueueFree();
    }
}