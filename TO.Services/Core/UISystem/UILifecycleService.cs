using Godot;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Repositories.Abstractions.Core.ResourceSystem;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Services.Core.UISystem;

/// <summary>
/// UI生命周期管理服务，负责UI组件的创建、缓存、销毁和状态管理
/// </summary>
public class UILifecycleService(IUIManagerRepo uiManagerRepo, IResourceLoaderRepo resourceLoaderRepo) : BaseService, IUILifecycleService
{
    /// <summary>
    /// 创建UI实例并注册到管理器
    /// </summary>
    /// <param name="path">UI场景路径</param>
    /// <returns>创建的UI实例</returns>
    public IUIScreen CreateUI(string path)
    {
        var packedScene = resourceLoaderRepo.LoadResource(path) as PackedScene
            ?? throw new InvalidOperationException($"Failed to load UI scene at {path}");

        var instance = packedScene.Instantiate<Node>();
        if (instance is not IUIScreen screen)
        {
            throw new InvalidCastException($"UI at {path} does not implement IUIScreen");
        }
        
        uiManagerRepo.RegisterScreen(screen);
        return screen;
    }

    /// <summary>
    /// 销毁UI实例并释放资源
    /// </summary>
    /// <param name="screen">要销毁的UI实例</param>
    /// <param name="path">UI场景路径</param>
    public void DestroyUI(IUIScreen? screen, string path)
    {
        if (screen == null) return;

        uiManagerRepo.UnregisterScreen(screen);
        resourceLoaderRepo.DecreaseReferenceCount(path);
    }

  
    public void DestroyAllUI()
    {
        // 获取所有已注册的屏幕副本，避免在遍历时修改集合
        var allScreens = uiManagerRepo.ScreensByName.Values.ToList();
        
        // 逐个注销所有屏幕
        foreach (var screen in allScreens)
        {
            uiManagerRepo.UnregisterScreen(screen);
        }
        
        // 清空历史记录
        uiManagerRepo.History?.Clear();
        
        // 重置当前屏幕
        uiManagerRepo.CurrentScreen = null;
        
        // 清理所有资源缓存
        resourceLoaderRepo.ClearCache();
    }
}
