namespace TO.Nodes.Abstractions.Nodes.UI.Bases;

public interface IUILayer
{
    void SetLayerName(string? layerName);
    
    string? LayerName { get; set; }

    void SetActive(bool active);

    void AddUIScreen(IUIScreen screen);

    void RemoveUIScreen(IUIScreen screen);
}