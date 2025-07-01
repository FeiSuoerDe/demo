using Autofac.Features.Indexed;
using GodotTask;
using TO.Apps.Events;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Configs;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Core.LogSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Services.Abstractions.Core.UISystem;

namespace TO.Apps.Services.Core.UISystem;

public class UIManagerAppService : BaseService
{
    private readonly IUIManagerRepo _uiManagerRepo;
    private readonly IUIManagerService _uiManagerService;
    private readonly IUILayerService _uiLayerService;
    private readonly IUILifecycleService _uiLifecycleService;
    private readonly ILoggerRepo _logger;
    private readonly IIndex<EventEnums,IEventBus> _eventBus;

    public UIManagerAppService(IUIManagerService uiManagerService,
        IUILayerService uiLayerService, 
        IUIManagerRepo uiManagerRepo, 
        IUILifecycleService uiLifecycleService, 
        ILoggerRepo logger,
        IIndex<EventEnums,IEventBus> eventBus
        )
    {
        _uiManagerService = uiManagerService;
        _uiLayerService = uiLayerService;
        _uiManagerRepo = uiManagerRepo;
        _uiLifecycleService = uiLifecycleService;
        _logger = logger;
        _eventBus = eventBus;
        _eventBus[EventEnums.UI].Subscribe<ShowUI>(e => ExecuteShowCommand(e.UIName)).AddTo(CancellationTokenSource.Token);
        _eventBus[EventEnums.UI].Subscribe<Hide>(_ => ExecuteHideCommand()).AddTo(CancellationTokenSource.Token);
        _eventBus[EventEnums.UI].Subscribe<HideAll>(_ => ExecuteHideAllCommand()).AddTo(CancellationTokenSource.Token);
        uiManagerRepo.Ready += () => ExecuteShowCommand(UIName.MainMenuScreen);
    }
    
    private void ExecuteShowCommand(UIName screenName)
    {
        var screen = _uiManagerRepo.GetScreenByName(screenName.ToString()) ?? _uiLifecycleService.CreateUI(UIConfigs.UIPaths[screenName]);
        _uiLayerService.HandleShowScreenLayerRelation(screen, _uiManagerRepo.CurrentScreen);
        _uiManagerService.Show(screen);
        _uiManagerRepo.CurrentScreen = screen;
    }

    private void ExecuteHideCommand()
    {
        _uiManagerService.CloseScreen();
    }

    private void ExecuteHideAllCommand()
    {
        _uiManagerService.HideScreens();
       
    }
    
}