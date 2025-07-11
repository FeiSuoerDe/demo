using TO.Nodes.Abstractions.UI.Bases;

namespace TO.Services.Abstractions.Core.UISystem;

/// <summary>
/// UI管理服务接口
/// </summary>
public interface IUIManagerService
{
    /// <summary>
    /// 显示指定名称的屏幕
    /// </summary>
    /// <param name="screenName">要显示的屏幕名称，不能为空或空白字符串</param>
    /// <param name="keepInHistory">是否将当前屏幕存入历史记录，默认为true</param>
    /// <remarks>
    /// 该方法应确保：
    /// 1. 根据名称查找屏幕实例
    /// 2. 正确处理屏幕层级关系
    /// 3. 更新Z索引
    /// 4. 管理历史记录
    /// </remarks>
    void Show(string screenName, bool keepInHistory = true);
    
    /// <summary>
    /// 显示指定的屏幕实例
    /// </summary>
    /// <param name="screen">要显示的屏幕实例</param>
    /// <param name="keepInHistory">是否将当前屏幕存入历史记录，默认为true</param>
    /// <remarks>
    /// 该方法应确保：
    /// 1. 正确处理屏幕层级关系
    /// 2. 更新Z索引
    /// 3. 管理历史记录
    /// </remarks>
    void Show(IUIScreen screen, bool keepInHistory = true);

    /// <summary>
    /// 关闭当前屏幕并显示历史记录中的上一个屏幕
    /// </summary>
    /// <remarks>
    /// 该方法应确保：
    /// 1. 从历史记录弹出上一个屏幕
    /// 2. 恢复正确的屏幕层级
    /// 3. 如果历史记录为空则不执行任何操作
    /// </remarks>
    void HideScreen();

    /// <summary>
    /// 隐藏所有屏幕并清空历史记录
    /// </summary>
    /// <remarks>
    /// 该方法应确保：
    /// 1. 隐藏所有可见屏幕
    /// 2. 清空历史记录
    /// 3. 重置当前屏幕引用
    /// </remarks>
    void HideAllScreens();
}
