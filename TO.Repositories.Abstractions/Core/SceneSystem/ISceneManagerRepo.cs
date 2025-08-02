using Godot;
using GodotTask;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Bases;

namespace TO.Repositories.Abstractions.Core.SceneSystem;

public interface ISceneManagerRepo : ISingletonNodeRepo<ISceneManager>
{
    CanvasLayer? CanvasLayer { get; set; }
    ColorRect? ColorRect { get; set; }
    Node? CurrentScene { get; set; }
    SceneTree? SceneTree { get; set; }
    
    /// <summary>
    /// 执行过渡效果
    /// </summary>
    /// <param name="effectType">效果类型</param>
    /// <param name="time">过渡时间</param>
    /// <param name="isEntering">是否为入场效果</param>
    GDTask ExecuteTransitionEffect(TransitionEffectType effectType, float time, bool isEntering);
    
    /// <summary>
    /// 打断当前过渡
    /// </summary>
    void InterruptTransition();
}