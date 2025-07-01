using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace TO.Domains.Services.Abstractions.Core.UISystem;

/// <summary>
/// UI生命周期管理服务接口
/// </summary>
public interface IUILifecycleService
{
    /// <summary>
    /// 创建UI实例
    /// </summary>
    /// <param name="path">UI场景路径</param>
    /// <param name="parent">父节点</param>
    /// <returns>创建的UI实例</returns>
    IUIScreen CreateUI(string path);

    /// <summary>
    /// 销毁UI实例
    /// </summary>
    /// <param name="screen">要销毁的UI实例</param>
    /// <param name="path">UI场景路径</param>
    void DestroyUI(IUIScreen? screen, string path);



}


