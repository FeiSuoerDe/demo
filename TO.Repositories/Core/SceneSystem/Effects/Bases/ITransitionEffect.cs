using GodotTask;

namespace TO.Repositories.Core.SceneSystem.Effects.Bases;

/// <summary>
/// 过渡效果接口
/// </summary>
public interface ITransitionEffect
{
    /// <summary>
    /// 执行过渡效果
    /// </summary>
    /// <param name="system">过渡系统</param>
    /// <returns>异步任务</returns>
    GDTask Execute(TransitionSystem system);
        
    /// <summary>
    /// 中断效果
    /// </summary>
    void Interrupt();
        
    /// <summary>
    /// 效果参数
    /// </summary>
    Dictionary<string, object> Parameters { get; set; }
}