using TO.Domains.Models.Repositories.Abstractions.Core.LogSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace Domains.Core.UISystem;

/// <summary>
/// UI管理服务，负责管理UI屏幕的显示、隐藏和层级关系
/// </summary>
public class UIManagerService(IUIManagerRepo uiManagerRepo,ILoggerRepo logger) : BasesService,IUIManagerService
{
    public void HideScreens()
    {
        uiManagerRepo.History?.Clear();
        
        foreach (var screen in uiManagerRepo.ScreensByName.Values)
        {
            screen.Hide();
        }
        
        logger.Info($"已隐藏所有屏幕");
    }

    public void CloseScreen()
    {
        var previousScreen = uiManagerRepo.PopFromHistory();
        if(previousScreen == null) return;

        uiManagerRepo.CurrentScreen?.Hide();
        previousScreen.Show();
        uiManagerRepo.CurrentScreen = previousScreen;
        
        logger.Info($"返回上一个屏幕: {previousScreen.GetType().Name}");

    }

    public void Show(IUIScreen screen, bool keepInHistory = true)
    {
        // 主菜单特殊处理
        if (keepInHistory && uiManagerRepo.CurrentScreen != null)
        {
            uiManagerRepo.PushToHistory(uiManagerRepo.CurrentScreen);
            uiManagerRepo.CurrentScreen = screen;
            screen.Show();  
        }
        
        logger.Info($"显示屏幕: {screen.GetType().Name}");
    }
    
  
}
