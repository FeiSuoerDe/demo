using TO.Commons.Enums.UI;

namespace TO.Commons.Configs;

/// <summary>
/// UI层级配置，定义层级之间的显示规则
/// </summary>
public class UILayerConfig
{
    /// <summary>
    /// 层级显示规则配置
    /// Key: 上层UI的层级
    /// Value: 对下层UI的影响配置
    /// </summary>
    private static readonly Dictionary<UILayerType, LayerBehavior> DefaultLayerBehaviors = new()
    {
        { UILayerType.System, new LayerBehavior(true) },     // 系统层级显示时隐藏所有下层
        { UILayerType.Alert, new LayerBehavior(true) },      // 警告层级显示时隐藏所有下层
        { UILayerType.Loading, new LayerBehavior(false) },   // 加载层级显示时不影响下层
        { UILayerType.Popup, new LayerBehavior(false) },     // 弹出层级显示时不影响下层
        { UILayerType.Dialog, new LayerBehavior(false) },    // 对话框层级显示时不影响下层
        { UILayerType.Normal, new LayerBehavior(true) },     // 普通层级显示时隐藏下层
        { UILayerType.Background, new LayerBehavior(false) } // 背景层级显示时不影响其他层
    };

    private static Dictionary<UILayerType, LayerBehavior> _currentLayerBehaviors;

    static UILayerConfig()
    {
        _currentLayerBehaviors = new Dictionary<UILayerType, LayerBehavior>(DefaultLayerBehaviors);
    }

    /// <summary>
    /// 获取指定层级是否应该隐藏下层UI
    /// </summary>
    /// <param name="layerType">要检查的层级</param>
    /// <returns>true表示应该隐藏下层UI，false表示不影响下层UI</returns>
    public static bool ShouldHideLowerLayers(UILayerType layerType)
    {
        return _currentLayerBehaviors.TryGetValue(layerType, out var behavior) && behavior.HideLowerLayers;
    }

    /// <summary>
    /// 更新层级行为配置
    /// </summary>
    /// <param name="layerType">要更新的层级</param>
    /// <param name="hideLowerLayers">是否隐藏下层UI</param>
    public static void UpdateLayerBehavior(UILayerType layerType, bool hideLowerLayers)
    {
        _currentLayerBehaviors[layerType] = new LayerBehavior(hideLowerLayers);
    }

    /// <summary>
    /// 重置为默认配置
    /// </summary>
    public static void ResetToDefault()
    {
        _currentLayerBehaviors = new Dictionary<UILayerType, LayerBehavior>(DefaultLayerBehaviors);
    }
}

/// <summary>
/// 层级行为配置
/// </summary>
public class LayerBehavior
{
    /// <summary>
    /// 是否隐藏下层UI
    /// </summary>
    public bool HideLowerLayers { get; }

    public LayerBehavior(bool hideLowerLayers)
    {
        HideLowerLayers = hideLowerLayers;
    }
}
