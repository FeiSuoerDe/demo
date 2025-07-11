using TO.Nodes.Abstractions.Bases;

namespace TO.Repositories.Abstractions.Bases;

public interface INodeRepo<out TNode> where TNode : class, INode
{
    event Action? Ready;
    event Action? TreeExiting;
    
    TNode? Node { get; }
}