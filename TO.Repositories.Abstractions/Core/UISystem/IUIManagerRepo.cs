using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.Singletons;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Repositories.Abstractions.Bases;

namespace TO.Repositories.Abstractions.Core.UISystem;

/// <summary>
/// UI管理仓储接口
/// 负责UI屏幕的数据存储、层级管理、历史记录维护等核心功能
/// 作为基础设施层的仓储接口，定义了UI数据持久化和检索的标准操作
/// </summary>
public interface IUIManagerRepo : ISingletonNodeRepo<IUIManager>
{
    /// <summary>
    /// 按名称存储的UI屏幕字典
    /// 键为屏幕类型名称，值为对应的UI屏幕实例
    /// </summary>
    Dictionary<string, IUIScreen> ScreensByName { get; }
    
    /// <summary>
    /// UI层级字典，管理不同层级的UI容器
    /// 键为层级类型，值为对应的UI层级实例
    /// </summary>
    Dictionary<UILayerType, IUILayer> UILayers { get; }
    
    /// <summary>
    /// UI屏幕历史记录栈
    /// 用于实现屏幕的前进后退功能
    /// </summary>
    Stack<IUIScreen?>? History { get; }
    
    /// <summary>
    /// 当前显示的UI屏幕
    /// 可能为null表示没有屏幕在显示
    /// </summary>
    IUIScreen? CurrentScreen { get; set;}

    /// <summary>
    /// 注册UI屏幕到管理器
    /// 将屏幕添加到名称字典和对应的UI层级中
    /// </summary>
    /// <param name="screen">要注册的UI屏幕实例</param>
    void RegisterScreen(IUIScreen? screen);
    
    /// <summary>
    /// 从管理器中注销UI屏幕
    /// 从名称字典和对应的UI层级中移除屏幕
    /// </summary>
    /// <param name="screen">要注销的UI屏幕实例</param>
    void UnregisterScreen(IUIScreen? screen);
    
    /// <summary>
    /// 将屏幕推入历史记录栈
    /// 用于实现屏幕导航的后退功能
    /// </summary>
    /// <param name="screen">要推入历史记录的屏幕</param>
    void PushToHistory(IUIScreen screen);
    
    /// <summary>
    /// 从历史记录栈中弹出最近的屏幕
    /// 用于实现屏幕导航的后退功能
    /// </summary>
    /// <returns>历史记录中的上一个屏幕，如果历史记录为空则返回null</returns>
    IUIScreen? PopFromHistory();

    /// <summary>
    /// 获取指定层级的所有屏幕
    /// </summary>
    /// <param name="layerType">UI层级类型</param>
    /// <returns>该层级中的所有屏幕的只读列表，如果层级不存在则返回空列表</returns>
    IReadOnlyList<IUIScreen> GetScreensInLayer(UILayerType layerType);

    /// <summary>
    /// 根据名称获取屏幕
    /// </summary>
    /// <param name="name">屏幕的类型名称</param>
    /// <returns>对应的UI屏幕实例，如果不存在则返回null</returns>
    IUIScreen? GetScreenByName(string name);
}