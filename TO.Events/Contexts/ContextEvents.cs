using Autofac;

namespace TO.Events.Contexts;

public static class ContextEvents
{
    public static event Action< object,Action<ILifetimeScope>>? RegisterNodeRepo;

    public static void TriggerRegisterNode(object o,Action<ILifetimeScope> builder)
    {
        RegisterNodeRepo?.Invoke(o,builder);
    }

    
}