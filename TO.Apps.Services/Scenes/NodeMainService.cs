using Autofac.Features.Indexed;
using Godot;
using TO.Apps.Events;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;

namespace TO.Apps.Services.Scenes;

public class NodeMainService
{
    private readonly IIndex<EventEnums,IEventBus> _eventBus;
    public NodeMainService(IIndex<EventEnums, IEventBus> eventBus)
    {
        _eventBus = eventBus;
        _eventBus[EventEnums.UI].Publish(new ShowUI(UIName.MainMenuScreen));
        GD.Print("NodeMainService: Initialized");
    }
}