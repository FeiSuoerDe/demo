using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.Singletons;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.UISystem;

/// <summary>
/// UI管理仓储实现类
/// </summary>
public class UIManagerRepo : SingletonNodeRepo<IUIManager>, IUIManagerRepo
{
    public Dictionary<string, IUIScreen> ScreensByName { get; } = new();
    public Dictionary<UILayerType, IUILayer> UILayers { get; } = [];
    public Stack<IUIScreen?>? History { get; private set; } = new();
    public IUIScreen? CurrentScreen { get; set;}
    
    protected override void Init()
    {
        // 初始化所有UI层级
        foreach (UILayerType layer in Enum.GetValues(typeof(UILayerType)))
        {
            var uiLayer = Singleton?.InitializeUILayer(layer.ToString());
            if (uiLayer != null) UILayers[layer] = uiLayer;
        }
        
        EmitReady();
    }
    
    public void RegisterScreen(IUIScreen? screen)
    {
        if (screen == null) return;

        ScreensByName[screen.GetType().Name] = screen;
        UILayers[screen.LayerType].AddUIScreen(screen);
    }

    public void UnregisterScreen(IUIScreen? screen)
    {
        if (screen == null) return;
        
        ScreensByName.Remove(screen.GetType().Name);
        UILayers[screen.LayerType].RemoveUIScreen(screen);
    }

    public IReadOnlyList<IUIScreen> GetScreensInLayer(UILayerType layerType)
    {
        return UILayers.TryGetValue(layerType, out var layer)
            ? layer.GetScreens()
            : new List<IUIScreen>().AsReadOnly();
    }

    public IUIScreen? GetScreenByName(string name)
    {
        return ScreensByName.GetValueOrDefault(name);
    }


    
    public void PushToHistory(IUIScreen screen)
    {
        ArgumentNullException.ThrowIfNull(screen);
        History ??= new Stack<IUIScreen?>();
        History.Push(screen);
    }

    public IUIScreen? PopFromHistory()
    {
        if (History == null || History.Count == 0) return null;
        return History.Pop();
    }
}
