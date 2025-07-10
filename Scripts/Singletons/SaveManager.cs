using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;

namespace demo.Singletons;

public partial class SaveManager : Node,ISaveManager
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        Contexts.Contexts.Instance.RegisterSingleNode<ISaveManager>(this);
    }

}