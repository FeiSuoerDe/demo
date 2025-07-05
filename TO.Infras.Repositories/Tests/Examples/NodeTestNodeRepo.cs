using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Test.Examples;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Tests.Examples;

namespace inFras.Tests.Examples;

public class NodeTestNodeRepo : NodeRepo<ITestNode>, INodeTestNodeRepo
{
    public NodeTestNodeRepo(ITestNode main)
    {
        Node = main;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
}