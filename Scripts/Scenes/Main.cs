using Autofac;
using Contexts;
using Godot;
using TO.Nodes.Abstractions.Scenes;
using TO.Repositories.Scenes;
using TO.Services.Scenes;

namespace demo.Scenes;

public partial class Main : Node, IMain
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        NodeScope = Contexts.Contexts.Instance.RegisterNode<IMain, NodeMainService>(this);
        // NodeScope = Contexts.Contexts.Instance.RegisterNode<IMain>()
    }

}