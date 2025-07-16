using Godot;
using MediatR;
using TO.Commands.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.SceneSystem;

namespace Apps.Core.GameProgress;

public class StartGameCommandHandler(ISceneManagerService sceneManagerService,
    IEventBusRepo iEventBusRepo) : IRequestHandler<StartGameCommand>
{
    public async Task Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        // iEventBusRepo[EventEnums.UI].Publish(new ShowUI(UIName.LoadingScreen));
        // await sceneManagerService.ChangeScene(SceneConfig.FarmScene);
        // iEventBusRepo[EventEnums.UI].Publish(new HideAllUI());
        // timeManagerService.StartTimeSystem(new GameTime(0,1,1,6,0));
        GD.Print("Starting game...");
    }
}