using System;
using System.Collections.Generic;
using Godot;
using TO.Nodes.Abstractions.UI.Bases;

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
    
    /// <summary>
    /// 获取当前层级中的所有屏幕
    /// </summary>
    /// <returns>屏幕列表</returns>
    public IReadOnlyList<IUIScreen> GetScreens()
    {
        var screens = new List<IUIScreen>();
        foreach (Node child in GetChildren())
        {
            if (child is IUIScreen screen)
            {
                screens.Add(screen);
            }
        }
        return screens.AsReadOnly();
    }
    

}