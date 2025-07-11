using Godot;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.Singletons;

public interface IAudioManager : INode
{
    Node? AudioNodeRoot { get; set; }
}