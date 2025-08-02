using GodotTask;
using TO.Commons.Enums.UI;

namespace TO.Services.Abstractions.Core.SceneSystem;

public interface ISceneManagerService
{
    
    /// <summary>
    /// 切换场景，支持指定过渡效果类型
    /// </summary>
    /// <param name="scenePath">场景路径</param>
    /// <param name="effectType">过渡效果类型</param>
    /// <param name="transitionTime">过渡时间</param>
    /// <returns>异步任务</returns>
    GDTask ChangeScene(string scenePath, TransitionEffectType effectType, float transitionTime = 0.5f);
}