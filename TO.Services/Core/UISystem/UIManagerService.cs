using GodotTask;
using TO.Commons.Configs;
using TO.Commons.Enums.UI;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Repositories.Abstractions.Core.LogSystem;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Services.Core.UISystem;

/// <summary>
/// UI管理服务，负责管理UI屏幕的显示、隐藏和层级关系
/// </summary>
public class UIManagerService : BaseService, IUIManagerService
{
    private readonly IEventBusRepo _iEventBusRepo;
    private readonly IUIManagerRepo _uiManagerRepo;
    private readonly IUILayerService _uiLayerService;
    private readonly IUILifecycleService _uiLifecycleService;
    private readonly ILoggerRepo _logger;

    /// <summary>
    /// UI管理服务，负责管理UI屏幕的显示、隐藏和层级关系
    /// </summary>
    public UIManagerService(IUIManagerRepo uiManagerRepo, IUILayerService uiLayerService, ILoggerRepo logger, IEventBusRepo iEventBusRepo, IUILifecycleService uiLifecycleService)
    {
        _uiManagerRepo = uiManagerRepo;
        _uiLayerService = uiLayerService;
        _logger = logger;
        _iEventBusRepo = iEventBusRepo;
        _uiLifecycleService = uiLifecycleService;
        _iEventBusRepo.Subscribe<ShowUI>(e => ExecuteShowCommand(e.UIName)).AddTo(CancellationTokenSource.Token);
        _iEventBusRepo.Subscribe<HideUI>(_ => ExecuteHideCommand()).AddTo(CancellationTokenSource.Token);
        _iEventBusRepo.Subscribe<HideAllUI>(_ => ExecuteHideAllCommand()).AddTo(CancellationTokenSource.Token);
        _iEventBusRepo.Subscribe<CloseUI>(e => ExecuteCloseCommand(e.UIName)).AddTo(CancellationTokenSource.Token);
        _iEventBusRepo.Subscribe<CloseAllUI>(_ => ExecuteCloseAllCommand()).AddTo(CancellationTokenSource.Token);
    }

    public void HideAllScreens()
    {
        _uiManagerRepo.History?.Clear();
        
        foreach (var screen in _uiManagerRepo.ScreensByName.Values)
        {
            screen.Hide();
        }
        
        _logger.Info($"已隐藏所有屏幕");
    }

    public void HideScreen()
    {
        var previousScreen = _uiManagerRepo.PopFromHistory();
        if(previousScreen == null) return;

        var currentScreen = _uiManagerRepo.CurrentScreen;
        currentScreen?.Hide();
        
        // 处理层级关系
        _uiLayerService.HandleShowScreenLayerRelation(previousScreen, currentScreen);
        
        previousScreen.Show();
        _uiManagerRepo.CurrentScreen = previousScreen;
        
        _logger.Info($"返回上一个屏幕: {previousScreen.GetType().Name}");
    }

    public void Show(string screenName, bool keepInHistory = true)
    {
        if (string.IsNullOrWhiteSpace(screenName))
        {
            _logger.Warning("屏幕名称不能为空或空白字符串");
            return;
        }
        
        var screen = _uiManagerRepo.GetScreenByName(screenName);
        if (screen == null)
        {
            _logger.Warning($"未找到名称为 '{screenName}' 的屏幕");
            return;
        }
        
        Show(screen, keepInHistory);
    }
    
    public void Show(IUIScreen screen, bool keepInHistory = true)
    {
        var currentScreen = _uiManagerRepo.CurrentScreen;
        
        // 如果需要保持历史记录且当前有屏幕，则推入历史栈
        if (keepInHistory && currentScreen != null)
        {
            _uiManagerRepo.PushToHistory(currentScreen);
        }
        
        // 处理层级关系
        _uiLayerService.HandleShowScreenLayerRelation(screen, currentScreen);
        
        // 设置当前屏幕并显示
        _uiManagerRepo.CurrentScreen = screen;
        screen.Show();
        
        _logger.Info($"显示屏幕: {screen.GetType().Name}");
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
        Show(screen);
    }

    private void ExecuteHideCommand() => HideScreen();

    private void ExecuteHideAllCommand() => HideAllScreens();
    
    private void ExecuteCloseCommand(UIName screenName)
    {
        if (!UIConfigs.UIPaths.TryGetValue(screenName, out var value))
        {
            _logger.Warning($"UI screen '{screenName}' not found in configuration");
            return;
        }
        var screen = _uiManagerRepo.GetScreenByName(screenName.ToString());
        if (screen != null)
        {
            _logger.Info($"Closing current UI screen: {screenName}");
            _uiLifecycleService.DestroyUI(screen, value);
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
