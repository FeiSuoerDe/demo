using Godot;
using GodotTask;
using inFras.Bases;
using inFras.Core.SceneSystem.Effects;
using inFras.Core.SceneSystem.Effects.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.SceneSystem;
using TO.Nodes.Abstractions.Nodes.Singletons;

namespace inFras.Core.SceneSystem;

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
        

    // 淡出效果
    public async GDTask FadeOut(float time = 1.0f)
    {
        if (_transitionSystem == null) return;
        var effect = new FadeTransitionEffect(false)
        {
            Parameters =
            {
                ["time"] = time
            }
        };

        _transitionSystem.EnqueueEffect(effect);
        await _transitionSystem.ExecuteAll();
    }

    // 淡入效果
    public async GDTask FadeIn(float time = 1.0f)
    {
        if (_transitionSystem == null)
        {
            return;
        }
        var effect = new FadeTransitionEffect(true)
        {
            Parameters =
            {
                ["time"] = time
            }
        };

        _transitionSystem.EnqueueEffect(effect);
        await _transitionSystem.ExecuteAll();
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