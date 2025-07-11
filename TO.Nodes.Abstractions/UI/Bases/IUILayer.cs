namespace TO.Nodes.Abstractions.UI.Bases;

public interface IUILayer
{
    void SetLayerName(string? layerName);
    
    string? LayerName { get; set; }

    void SetActive(bool active);

    void AddUIScreen(IUIScreen screen);

    void RemoveUIScreen(IUIScreen screen);
    
    /// <summary>
    /// 获取当前层级中的所有屏幕
    /// </summary>
    /// <returns>屏幕列表</returns>
    IReadOnlyList<IUIScreen> GetScreens();
  
}