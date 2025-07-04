using Autofac;
using Contexts;
using Godot;
using inFras.Tests.Examples;
using TO.Nodes.Abstractions.Tests.Examples;

namespace demo.Tests.Examples;

[GlobalClass]
public partial class TestNode : Node, ITestNode
{
    public ILifetimeScope? NodeScope { get; set; }
    
    public override void _Ready()
    {
        NodeScope = NodeContexts.Instance.RegisterNode<ITestNode, NodeTestNodeRepo>(this);
    }
}