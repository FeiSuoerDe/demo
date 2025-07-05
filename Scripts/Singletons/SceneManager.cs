using Autofac;
using Godot;
using TO.Nodes.Abstractions.Nodes.Singletons;

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
        Contexts.Contexts.Instance.RegisterNode<ISceneManager>(this);
    }
    
}