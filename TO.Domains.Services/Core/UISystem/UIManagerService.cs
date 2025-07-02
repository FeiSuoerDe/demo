using TO.Domains.Models.Repositories.Abstractions.Core.LogSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace Domains.Core.UISystem;

/// <summary>
/// UI管理服务，负责管理UI屏幕的显示、隐藏和层级关系
/// </summary>
public class UIManagerService(IUIManagerRepo uiManagerRepo, IUILayerService uiLayerService, ILoggerRepo logger) : BasesService, IUIManagerService
{
    public void HideAllScreens()
    {
        uiManagerRepo.History?.Clear();
        
        foreach (var screen in uiManagerRepo.ScreensByName.Values)
        {
            screen.Hide();
        }
        
        logger.Info($"已隐藏所有屏幕");
    }

    public void HideScreen()
    {
        var previousScreen = uiManagerRepo.PopFromHistory();
        if(previousScreen == null) return;

        var currentScreen = uiManagerRepo.CurrentScreen;
        currentScreen?.Hide();
        
        // 处理层级关系
        uiLayerService.HandleShowScreenLayerRelation(previousScreen, currentScreen);
        
        previousScreen.Show();
        uiManagerRepo.CurrentScreen = previousScreen;
        
        logger.Info($"返回上一个屏幕: {previousScreen.GetType().Name}");
    }

    public void Show(string screenName, bool keepInHistory = true)
    {
        if (string.IsNullOrWhiteSpace(screenName))
        {
            logger.Warning("屏幕名称不能为空或空白字符串");
            return;
        }
        
        var screen = uiManagerRepo.GetScreenByName(screenName);
        if (screen == null)
        {
            logger.Warning($"未找到名称为 '{screenName}' 的屏幕");
            return;
        }
        
        Show(screen, keepInHistory);
    }
    
    public void Show(IUIScreen screen, bool keepInHistory = true)
    {
        if (screen == null)
        {
            logger.Warning("屏幕实例不能为null");
            return;
        }
        
        var currentScreen = uiManagerRepo.CurrentScreen;
        
        // 如果需要保持历史记录且当前有屏幕，则推入历史栈
        if (keepInHistory && currentScreen != null)
        {
            uiManagerRepo.PushToHistory(currentScreen);
        }
        
        // 处理层级关系
        uiLayerService.HandleShowScreenLayerRelation(screen, currentScreen);
        
        // 设置当前屏幕并显示
        uiManagerRepo.CurrentScreen = screen;
        screen.Show();
        
        logger.Info($"显示屏幕: {screen.GetType().Name}");
    }
    
  
}
