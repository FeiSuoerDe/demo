using MediatR;
using TO.Commands.Core;
using TO.Commons.Enums.UI;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.SceneSystem;
using TO.Services.Bases;

namespace TO.Services.UI.Screens;

public class NodeMainMenuScreenService: BaseService
{
    private readonly IMainMenuScreen _mainMenuScreen;
    private readonly ISceneManagerService _sceneManagerService;
    private readonly IMediator _mediator;
    private readonly IEventBusRepo  _iEventBusRepo;

    public NodeMainMenuScreenService(IMainMenuScreen mainMenuScreen,
        IEventBusRepo eventBus, ISceneManagerService sceneManagerService, IMediator mediator)
    {
        _mainMenuScreen = mainMenuScreen;
        _sceneManagerService = sceneManagerService;
        _mediator = mediator;
        _iEventBusRepo = eventBus;
       
        _mainMenuScreen.OnStartButtonPressed += OnStartButtonPressed;
        _mainMenuScreen.OnSettingsButtonPressed += OnSettingsButtonPressed;
        _mainMenuScreen.OnExitButtonPressed += OnExitButtonPressed;
    }

    
    private void OnStartButtonPressed()
    {
        _mediator.Send(new StartGameCommand());
    }

    private void OnSettingsButtonPressed()
    {
        _iEventBusRepo.Publish(new ShowUI(UIName.SettingsMenuScreen));
    }
    private  void OnExitButtonPressed()
    {
         _mediator.Send(new QuitGameCommand());
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _mainMenuScreen.OnStartButtonPressed -= OnStartButtonPressed;
        _mainMenuScreen.OnSettingsButtonPressed -= OnSettingsButtonPressed;
        _mainMenuScreen.OnExitButtonPressed -= OnExitButtonPressed;
    }

    
}