using Godot;
using Autofac;
using Contexts;
using inFras.Scenes;
using TO.Nodes.Abstractions.Nodes.Scenes;

public partial class Main : Node, IMain
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        NodeScope = NodeContexts.Instance.RegisterNode<IMain, NodeMainRepo>(this);
    }

}
