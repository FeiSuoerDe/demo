using TO.GodotNodes.Abstractions;

namespace TO.Domains.Models.Repositories.Abstractions.Bases;

public interface INodeRepo<out TNode> where TNode : class, INode
{
    event Action? Ready;
    event Action? TreeExiting;
    
    TNode? Node { get; }
}