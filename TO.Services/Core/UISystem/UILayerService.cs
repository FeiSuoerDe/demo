using TO.Commons.Configs;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Services.Core.UISystem;

/// <summary>
/// UI层级管理服务实现类
/// </summary>
public class UILayerService(IUIManagerRepo uiManagerRepo) : BaseService, IUILayerService
{
    /// <summary>
    /// 处理屏幕层级关系
    /// </summary>
    public void HandleShowScreenLayerRelation(IUIScreen newScreen, IUIScreen? currentScreen)
    {
        if (currentScreen == null) return;
        
        // 如果在同一层级且新屏幕不透明，则隐藏当前屏幕
        if (currentScreen.LayerType == newScreen.LayerType && !newScreen.IsTransparent)
        {
            currentScreen.Hide();
        }
        
        // 检查新屏幕的层级是否需要隐藏下层UI
        if (ShouldHideLowerLayers(newScreen.LayerType))
        {
            HideLowerLayers(newScreen.LayerType);
        }
    }
    
    /// <summary>
    /// 检查指定层级是否应该隐藏低层级UI
    /// </summary>
    public bool ShouldHideLowerLayers(UILayerType layerType)
    {
        return UILayerConfig.ShouldHideLowerLayers(layerType);
    }
    
    /// <summary>
    /// 显示指定层级的所有UI
    /// </summary>
    public void ShowLayer(UILayerType layerType)
    {
        if (uiManagerRepo.UILayers.TryGetValue(layerType, out var layer))
        {
            layer.SetActive(true);
            
        }
    }
    
    /// <summary>
    /// 隐藏指定层级的所有UI
    /// </summary>
    public void HideLayer(UILayerType layerType)
    {
        if (uiManagerRepo.UILayers.TryGetValue(layerType, out var layer))
        {
            layer.SetActive(false);
            
        }
    }
    
    /// <summary>
    /// 设置指定层级的可见性
    /// </summary>
    public void SetLayerVisible(UILayerType layerType, bool visible)
    {
        if (visible)
        {
            ShowLayer(layerType);
        }
        else
        {
            HideLayer(layerType);
        }
    }
    
    /// <summary>
    /// 更新指定层级的Z索引
    /// </summary>
    public void UpdateLayerZIndex(UILayerType layerType)
    {
        if (uiManagerRepo.UILayers.TryGetValue(layerType, out var layer))
        {
            // 设置层级的Z索引为枚举值
            if (layer is Godot.CanvasLayer canvasLayer)
            {
                canvasLayer.Layer = (int)layerType;
            }
        }
    }
    
    /// <summary>
    /// 更新所有可见屏幕的Z索引
    /// </summary>
    public void UpdateAllScreensZIndex()
    {
        // 更新所有层级的Z索引
        foreach (var layerType in System.Enum.GetValues<UILayerType>())
        {
            UpdateLayerZIndex(layerType);
        }
    }
    
    /// <summary>
    /// 隐藏指定层级以下的所有层级
    /// </summary>
    private void HideLowerLayers(UILayerType currentLayerType)
    {
        var currentLayerValue = (int)currentLayerType;
        
        foreach (var layerType in System.Enum.GetValues<UILayerType>())
        {
            var layerValue = (int)layerType;
            
            // 隐藏所有低于当前层级的层级
            if (layerValue < currentLayerValue)
            {
                HideLayer(layerType);
            }
        }
    }
}
