using System;
using Godot;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace demo.UI.Bases;

public partial class UILayer : CanvasLayer, IUILayer
{
    public string? LayerName { get; set; }

    public void SetLayerName(string? layerName)
    {
        LayerName = layerName ?? throw new ArgumentNullException(nameof(layerName));
        Name = layerName;
    }
    
    public void SetActive(bool active)
    {
        Visible = active;
    }
    
    public void AddUIScreen(IUIScreen screen)
    {
        AddChild((UIScreen)screen);
    }
    
    public void RemoveUIScreen(IUIScreen screen)
    {
        var child = (UIScreen)screen;
        RemoveChild(child);
        child.QueueFree();
    }
}