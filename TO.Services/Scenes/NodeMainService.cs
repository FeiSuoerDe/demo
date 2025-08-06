using Godot;
using TO.Commons.Enums.UI;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Bases;

namespace TO.Services.Scenes;

public class NodeMainService : BaseService
{
    private readonly IEventBusRepo _iEventBusRepo;
    public NodeMainService(IEventBusRepo iEventBusRepo)
    {
        _iEventBusRepo = iEventBusRepo;
        _iEventBusRepo.Publish(new ShowUI(UIName.MainMenuScreen));
        GD.Print("NodeMainService: Initialized");
    }
}