using Autofac;
using Godot;
using TO.Nodes.Abstractions.Tests.Examples;

namespace demo.Tests.Examples;

public partial class TestManager : Node, ITestManager
{
    public ILifetimeScope? NodeScope { get; set; }
    
    public override void _Ready()
    {
        Contexts.Contexts.Instance.RegisterNode<ITestManager>(this);
    }
}