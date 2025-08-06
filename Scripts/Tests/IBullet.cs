using demo.Core.GameAbilitySystem;

namespace demo.Tests;

public interface IBullet
{
    void Hit(AbilitySystemComponent asc);
    void Destroy();
}