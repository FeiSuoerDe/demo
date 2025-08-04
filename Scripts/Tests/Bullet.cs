using demo.Core.GameAbilitySystem;
using Godot;

namespace demo.Tests;

public partial class Bullet : Node2D, IBullet
{
    [Export]
    public string _effectID;
    
    [Export]
    private Area2D _area;

    private Vector2 _direction;

    private float lifeTime = 5f;
    public void Init(Vector2 direction)
    {
        _direction = direction;
        ProcessMode = ProcessModeEnum.Always;
    }
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Disabled;
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
        asc.ApplyEffect(_effectID,null);
    }

    public void Destroy()
    {
        QueueFree();
    }
}