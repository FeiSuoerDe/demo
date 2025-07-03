using Apps.Commands.Core;
using Autofac.Features.Indexed;
using Godot;
using GodotTask;
using MediatR;
using TO.Apps.Events;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Enums;
using TO.Domains.Eevents;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Domains.Services.Abstractions.Core.SceneSystem;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeMainMenuScreenService: BaseService
{
    private readonly IMainMenuScreenRepo _mainMenuScreenRepo;
    private readonly ISceneManagerService _sceneManagerService;
    private readonly IMediator _mediator;
    private readonly IEventBus  _eventBus;

    public NodeMainMenuScreenService(IMainMenuScreenRepo mainMenuScreenRepo,
        IIndex<EventEnums,IEventBus> eventBus, ISceneManagerService sceneManagerService, IMediator mediator)
    {
        _mainMenuScreenRepo = mainMenuScreenRepo;
        _sceneManagerService = sceneManagerService;
        _mediator = mediator;
        _eventBus = eventBus[EventEnums.UI];
       
        _mainMenuScreenRepo.StartButtonPressed += OnStartButtonPressed;
        _mainMenuScreenRepo.SettingsButtonPressed += OnSettingsButtonPressed;
        _mainMenuScreenRepo.ExitButtonPressed += OnExitButtonPressed;
    }

    
    private void OnStartButtonPressed()
    {
        _mediator.Send(new StartGameCommand());
    }

    private void OnSettingsButtonPressed()
    {
        _eventBus.Publish(new ShowUI(UIName.SettingsMenuScreen));
    }
    private void OnExitButtonPressed()
    {
        _mediator.Send(new QuitGameCommand());
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _mainMenuScreenRepo.StartButtonPressed -= OnStartButtonPressed;
        _mainMenuScreenRepo.SettingsButtonPressed -= OnSettingsButtonPressed;
        _mainMenuScreenRepo.ExitButtonPressed -= OnExitButtonPressed;
    }

    
}