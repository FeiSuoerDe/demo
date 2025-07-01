using inFras.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.Singletons;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace inFras.Core.UISystem;

/// <summary>
/// UI管理仓储，仅负责UI屏幕数据存储和初始化
/// </summary>
public class UIManagerRepo : SingletonNodeRepo<IUIManager>, IUIManagerRepo
{
    /// <summary>
    /// 按名称存储的UI屏幕字典
    /// </summary>
    public Dictionary<string, IUIScreen> ScreensByName { get; } = new();

    /// <summary>
    /// 按层级存储的UI屏幕字典
    /// </summary>
    public Dictionary<UILayerType, List<IUIScreen>> ScreensByLayer { get; } = new();
    
    public Dictionary<UILayerType, IUILayer> UILayers { get; } = [];

    public Stack<IUIScreen?>? History { get; private set; } = new();
    
    public IUIScreen? CurrentScreen { get; set;}
    
    protected override void Init()
    {
        // 初始化层级字典
        foreach (UILayerType layer in Enum.GetValues(typeof(UILayerType)))
        {
            var uiLayer = Singleton?.InitializeUILayer(layer.ToString());
            if (uiLayer != null) UILayers[layer] = uiLayer;
        }
        
        EmitReady();
    }
    
    /// <summary>
    /// 注册UI屏幕到管理器
    /// </summary>
    public void RegisterScreen(IUIScreen? screen)
    {
        if (screen == null) return;

        // 添加到名称字典
        ScreensByName[screen.GetType().Name] = screen;
        UILayers[screen.LayerType].AddUIScreen(screen);
    }

    public void UnregisterScreen(IUIScreen? screen)
    {
        if (screen == null) return;
        
        ScreensByName.Remove(screen.GetType().Name);
        UILayers[screen.LayerType].RemoveUIScreen(screen);
    }

    /// <summary>
    /// 获取指定层级的所有屏幕
    /// </summary>
    public IReadOnlyList<IUIScreen> GetScreensInLayer(UILayerType layerType)
    {
        return ScreensByLayer.TryGetValue(layerType, out var screens)
            ? screens.AsReadOnly()
            : new List<IUIScreen>().AsReadOnly();
    }

    /// <summary>
    /// 根据名称获取屏幕
    /// </summary>
    public IUIScreen? GetScreenByName(string name)
    {
        return ScreensByName.GetValueOrDefault(name);
    }

    public void AddInScreensByLayer(IUIScreen screen)
    {
        
        if(!ScreensByLayer.TryGetValue(screen.LayerType, out _))
        {
            ScreensByLayer[screen.LayerType] = [screen];
        }
        else if(!ScreensByLayer[screen.LayerType].Contains(screen))
        {
            ScreensByLayer[screen.LayerType].Add(screen);
        }
    }
    
    public void RemoveFromScreensByLayer(IUIScreen screen)
    {
        if(!ScreensByLayer.TryGetValue(screen.LayerType, out var screens)) return;
        screens.Remove(screen);
    }
    
    public void PushToHistory(IUIScreen screen)
    {
        if (screen == null) throw new ArgumentNullException(nameof(screen));
        History ??= new Stack<IUIScreen?>();
        History.Push(screen);
    }

    public IUIScreen? PopFromHistory()
    {
        if (History == null || History.Count == 0) return null;
        return History.Pop();
    }
}
