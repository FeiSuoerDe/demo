using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Bases;
using TO.Nodes.Abstractions.Nodes.Singletons;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace TO.Domains.Models.Repositories.Abstractions.Core.UISystem;

/// <summary>
/// UI管理仓储接口，仅提供UI屏幕数据存储
/// </summary>
public interface IUIManagerRepo : ISingletonNodeRepo<IUIManager>
{
    /// <summary>
    /// 按名称存储的UI屏幕字典
    /// </summary>
    Dictionary<string, IUIScreen> ScreensByName { get; }
    
    /// <summary>
    /// 按层级存储的UI屏幕字典
    /// </summary>
    Dictionary<UILayerType, List<IUIScreen>> ScreensByLayer { get; }
    
    Stack<IUIScreen?>? History { get; }
    
    IUIScreen? CurrentScreen { get; set;}


    void RegisterScreen(IUIScreen? screen);
    void UnregisterScreen(IUIScreen? screen);
    void PushToHistory(IUIScreen screen);
    
    IUIScreen? PopFromHistory();
    
    

    void AddInScreensByLayer(IUIScreen screen);

    void RemoveFromScreensByLayer(IUIScreen screen);

    /// <summary>
    /// 获取指定层级的所有屏幕
    /// </summary>
    IReadOnlyList<IUIScreen> GetScreensInLayer(UILayerType layerType);

    /// <summary>
    /// 根据名称获取屏幕
    /// </summary>
    IUIScreen? GetScreenByName(string name);
}