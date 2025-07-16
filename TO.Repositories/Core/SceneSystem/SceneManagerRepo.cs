using Godot;
using GodotTask;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Core.SceneSystem;
using TO.Repositories.Bases;
using TO.Repositories.Core.SceneSystem.Effects;
using TO.Repositories.Core.SceneSystem.Effects.Bases;

namespace TO.Repositories.Core.SceneSystem;

public class SceneManagerRepo : SingletonNodeRepo<ISceneManager>, ISceneManagerRepo
{
    public CanvasLayer? CanvasLayer { get; set; }
    public ColorRect? ColorRect { get; set; }
    public Node? CurrentScene { get; set; }
    public SceneTree? SceneTree { get; set; }
        
    private TransitionSystem? _transitionSystem;
    protected override void Init()
    {
        CanvasLayer = Singleton?.CanvasLayer;
        ColorRect = Singleton?.ColorRect;
        base.Init();
        EmitReady();
        SceneTree = Engine.GetMainLoop() as SceneTree;
        CurrentScene = SceneTree?.CurrentScene;

        if (CanvasLayer == null || ColorRect == null) return;
        _transitionSystem = new TransitionSystem(CanvasLayer, ColorRect,SceneTree);
        _transitionSystem.OnStateChanged += OnTransitionStateChanged;
    }

    private void OnTransitionStateChanged(TransitionState state)
    {
        // 可以在这里添加状态变化的回调处理
        GD.Print($"Transition state changed to: {state}");
    }
    
    /// <summary>
    /// 执行过渡效果的通用方法
    /// </summary>
    /// <param name="effectType">效果类型</param>
    /// <param name="time">过渡时间</param>
    /// <param name="isEntering">是否为入场效果</param>
    public async GDTask ExecuteTransitionEffect(TransitionEffectType effectType, float time, bool isEntering)
    {
        if (_transitionSystem == null) return;
        
        // 尝试从注册系统创建效果
        var effect = TransitionEffectRegistry.CreateEffect(effectType, isEntering);
        if (effect != null)
        {
            effect.Parameters["time"] = time;
            await ExecuteEffectChain(effect);
        }
        else
        {
            // 回退到默认淡入淡出效果
            var fadeEffect = new FadeTransitionEffect(isEntering)
            {
                Parameters = { ["time"] = time }
            };
            await ExecuteEffectChain(fadeEffect);
        }
    }

    // 打断当前过渡
    public void InterruptTransition()
    {
        _transitionSystem?.Interrupt();
    }
    /// <summary>
    /// 注册自定义过渡效果
    /// </summary>
    /// <param name="effect">效果实例</param>
    public void RegisterEffect(ITransitionEffect effect)
    {
        _transitionSystem?.EnqueueEffect(effect);
    }

    /// <summary>
    /// 执行所有已注册的过渡效果
    /// </summary>
    public async GDTask ExecuteEffects()
    {
        if (_transitionSystem == null) return;
        await _transitionSystem.ExecuteAll();
    }

    /// <summary>
    /// 创建并执行一个效果链
    /// </summary>
    /// <param name="effects">效果数组</param>
    public async GDTask ExecuteEffectChain(params ITransitionEffect[] effects)
    {
        if (_transitionSystem == null) return;
            
        foreach (var effect in effects)
        {
            _transitionSystem.EnqueueEffect(effect);
        }
            
        await _transitionSystem.ExecuteAll();
    }
       
}