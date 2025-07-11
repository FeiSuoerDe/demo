using TO.Nodes.Abstractions.Scenes;
using TO.Repositories.Abstractions.Scenes;
using TO.Repositories.Bases;

namespace TO.Repositories.Scenes;

public class NodeMainRepo : NodeRepo<IMain>, INodeMainRepo
{
    public NodeMainRepo(IMain main)
    {
        Node = main;
        Register();
       
    }
}