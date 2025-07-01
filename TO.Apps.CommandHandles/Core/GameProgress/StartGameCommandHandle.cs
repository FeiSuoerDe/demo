using Apps.Commands.Core;
using Autofac.Features.Indexed;
using Godot;
using Infras.Writers;
using MediatR;
using TO.Apps.Events;
using TO.Commons.Configs;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Services.Abstractions.Core.SceneSystem;
using TO.Domains.Services.Abstractions.Core.TimeSystem;

namespace Apps.Core.GameProgress;

public class StartGameCommandHandle(ISceneManagerService sceneManagerService,
    ITimeManagerService  timeManagerService,
    IIndex<EventEnums,IEventBus> eventBus) : IRequestHandler<StartGameCommand>
{
    public async Task Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        // eventBus[EventEnums.UI].Publish(new ShowUI(UIName.LoadingScreen));
        // await sceneManagerService.ChangeScene(SceneConfig.FarmScene);
        // eventBus[EventEnums.UI].Publish(new HideAll());
        // timeManagerService.StartTimeSystem(new GameTime(0,1,1,6,0));
        GD.Print("Starting game...");
    }
}