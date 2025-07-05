using Autofac;
using Godot;
using TO.Nodes.Abstractions.Nodes.Singletons;

namespace demo.Singletons;

public partial class SaveManager : Node,ISaveManager
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        Contexts.Contexts.Instance.RegisterNode<ISaveManager>(this);
    }

}