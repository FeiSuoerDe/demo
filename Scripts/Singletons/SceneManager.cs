using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;

namespace TimelapseInvoices.Scripts.Singletons;


public partial class SceneManager : Node, ISceneManager
{
    [Export]
    public CanvasLayer? CanvasLayer { get; set; }
    
    [Export]
    public ColorRect? ColorRect { get; set; }

    public ILifetimeScope? NodeScope { get; set; }
    
    public override void _Ready()
    {
        TO.Contexts.Contexts.Instance.RegisterSingleNode<ISceneManager>(this);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
    
}