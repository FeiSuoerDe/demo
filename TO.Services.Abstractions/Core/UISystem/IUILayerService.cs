using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.UI.Bases;

namespace TO.Services.Abstractions.Core.UISystem;

/// <summary>
/// UI层级管理服务接口，负责处理UI屏幕的层级关系和显示规则
/// </summary>
public interface IUILayerService
{
    /// <summary>
    /// 处理屏幕层级关系
    /// </summary>
    /// <param name="newScreen">新显示的屏幕</param>
    /// <param name="currentScreen">当前屏幕（可为null）</param>
    void HandleShowScreenLayerRelation(IUIScreen newScreen, IUIScreen? currentScreen);
    
    /// <summary>
    /// 检查指定层级是否应该隐藏低层级UI
    /// </summary>
    /// <param name="layerType">要检查的层级</param>
    /// <returns>是否应该隐藏低层级</returns>
    bool ShouldHideLowerLayers(UILayerType layerType);
    
    /// <summary>
    /// 显示指定层级的所有UI
    /// </summary>
    /// <param name="layerType">要显示的层级</param>
    void ShowLayer(UILayerType layerType);
    
    /// <summary>
    /// 隐藏指定层级的所有UI
    /// </summary>
    /// <param name="layerType">要隐藏的层级</param>
    void HideLayer(UILayerType layerType);
    
    /// <summary>
    /// 设置指定层级的可见性
    /// </summary>
    /// <param name="layerType">目标层级</param>
    /// <param name="visible">是否可见</param>
    void SetLayerVisible(UILayerType layerType, bool visible);
    
    /// <summary>
    /// 更新指定层级的Z索引
    /// </summary>
    /// <param name="layerType">要更新的层级</param>
    void UpdateLayerZIndex(UILayerType layerType);
    
    /// <summary>
    /// 更新所有可见屏幕的Z索引
    /// </summary>
    void UpdateAllScreensZIndex();
}
