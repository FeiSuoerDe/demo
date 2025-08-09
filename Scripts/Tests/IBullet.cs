using TimelapseInvoices.Scripts.Core.GameAbilitySystem;

namespace TimelapseInvoices.Scripts.Tests;

public interface IBullet
{
    void Hit(AbilitySystemComponent asc);
    void Destroy();
}