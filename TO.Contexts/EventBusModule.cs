using Autofac;
using inFras.Core.EventBus;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;


namespace Contexts;

public class EventBusModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<EventBusR3>()
            .Keyed<IEventBus>(EventEnums.Domains).SingleInstance();
        builder.RegisterType<EventBusR3>()
            .Keyed<IEventBus>(EventEnums.Apps).SingleInstance();
        builder.RegisterType<EventBusR3>()
            .Keyed<IEventBus>(EventEnums.UI).SingleInstance();
    }
}