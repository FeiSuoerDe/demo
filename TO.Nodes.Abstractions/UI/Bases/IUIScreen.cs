using TO.Commons.Enums.UI;

namespace TO.Nodes.Abstractions.UI.Bases;

/// <summary>
/// UI屏幕接口，定义UI屏幕的基本功能
/// </summary>
public interface IUIScreen
{
    
    /// <summary>
    /// 是否是透明屏幕（透明屏幕不会自动隐藏下层UI）
    /// </summary>
    bool IsTransparent { get; set; }
    
    /// <summary>
    /// UI层级
    /// </summary>
    UILayerType LayerType { get; set; }
    
    
    
    /// <summary>
    /// 当前屏幕是否处于活动状态
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Z轴索引，用于控制UI的显示层级
    /// </summary>
    int ZIndex { get; set; }


    
    /// <summary>
    /// 屏幕状态改变时触发（包括显示/隐藏）
    /// </summary>
    event Action<bool>? OnVisibilityChanged;

    /// <summary>
    /// 显示屏幕
    /// </summary>
    void Show();
    
    /// <summary>
    /// 隐藏屏幕
    /// </summary>
    void Hide();
    
    /// <summary>
    /// 切换屏幕显示状态
    /// </summary>
    void Toggle();
}
