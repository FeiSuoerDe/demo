using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;

namespace demo.Singletons;


public partial class SceneManager : Node, ISceneManager
{
    [Export]
    public CanvasLayer? CanvasLayer { get; set; }
    
    [Export]
    public ColorRect? ColorRect { get; set; }

    public ILifetimeScope? NodeScope { get; set; }
    
    public override void _Ready()
    {
        Contexts.Contexts.Instance.RegisterSingleNode<ISceneManager>(this);
    }
    
}