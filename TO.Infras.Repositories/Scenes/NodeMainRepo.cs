using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Scenes;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.Scenes;

namespace inFras.Scenes;

public class NodeMainRepo : NodeRepo<IMain>, INodeMainRepo
{
    public NodeMainRepo(IMain main)
    {
        Node = main;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
}