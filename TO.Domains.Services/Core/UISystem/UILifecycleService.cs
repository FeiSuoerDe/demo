using Godot;
using TO.Domains.Models.Repositories.Abstractions.Core.ResourceSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace Domains.Core.UISystem;

/// <summary>
/// UI生命周期管理服务，负责UI组件的创建、缓存、销毁和状态管理
/// </summary>
public class UILifecycleService(IUIManagerRepo uiManagerRepo, IResourceLoaderRepo resourceLoaderRepo) : BasesService, IUILifecycleService
{
    /// <summary>
    /// 创建UI实例，支持缓存复用
    /// </summary>
    /// <param name="path">UI场景路径</param>
    /// <param name="parent">父节点</param>
    /// <returns>创建的UI实例</returns>
    public IUIScreen CreateUI(string path)
    {
        var packedScene = resourceLoaderRepo.LoadResource(path) as PackedScene;

        var instance = packedScene?.Instantiate<Node>();
        if (instance is not IUIScreen screen)
        {
            throw new InvalidCastException($"UI at {path} does not implement IUIScreen");
        }
        
        uiManagerRepo.RegisterScreen(screen);
        return screen;
    }

    /// <summary>
    /// 销毁UI实例，处理资源释放
    /// </summary>
    /// <param name="screen">要销毁的UI实例</param>
    /// <param name="path">UI场景路径</param>
    public void DestroyUI(IUIScreen? screen, string path)
    {
        if (screen == null) return;

        uiManagerRepo.UnregisterScreen(screen);

        resourceLoaderRepo.DecreaseReferenceCount(path);
    }
    
}
