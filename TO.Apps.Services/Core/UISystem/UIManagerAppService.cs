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
        _eventBus[EventEnums.UI].Subscribe<HideUI>(_ => ExecuteHideUICommand()).AddTo(CancellationTokenSource.Token);
        _eventBus[EventEnums.UI].Subscribe<HideAllUI>(_ => ExecuteHideAllUICommand()).AddTo(CancellationTokenSource.Token);
        _eventBus[EventEnums.UI].Subscribe<CloseUI>(e => ExecuteCloseCommand(e.UIName)).AddTo(CancellationTokenSource.Token);
        _eventBus[EventEnums.UI].Subscribe<CloseAllUI>(_ => ExecuteCloseAllCommand()).AddTo(CancellationTokenSource.Token);
        uiManagerRepo.Ready += () => ExecuteShowCommand(UIName.MainMenuScreen);
    }
    
    private void ExecuteShowCommand(UIName screenName)
    {
        if (!UIConfigs.UIPaths.TryGetValue(screenName, out var path))
        {
            _logger.Warning($"UI screen '{screenName}' not found in configuration");
            return;
        }
        var screen = _uiManagerRepo.GetScreenByName(screenName.ToString()) ?? _uiLifecycleService.CreateUI(path);
        _uiLayerService.HandleShowScreenLayerRelation(screen, _uiManagerRepo.CurrentScreen);
        _uiManagerService.Show(screen);
    }

    private void ExecuteHideUICommand() => _uiManagerService.HideScreen();

    private void ExecuteHideAllUICommand() => _uiManagerService.HideAllScreens();
    
    private void ExecuteCloseCommand(UIName screenName)
    {
        if (!UIConfigs.UIPaths.ContainsKey(screenName))
        {
            _logger.Warning($"UI screen '{screenName}' not found in configuration");
            return;
        }
        var screen = _uiManagerRepo.GetScreenByName(screenName.ToString());
        if (screen != null)
        {
            _logger.Info($"Closing current UI screen: {screenName}");
            _uiLifecycleService.DestroyUI(screen, UIConfigs.UIPaths[screenName]);
        }
        else
        {
            _logger.Warning($"Current screen is not '{screenName}', cannot close");
        }
    }
    
    private void ExecuteCloseAllCommand()
    {
        _uiLifecycleService.DestroyAllUI();
    }
    
}