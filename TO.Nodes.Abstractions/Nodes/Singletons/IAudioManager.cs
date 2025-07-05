using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.Singletons;

public interface IAudioManager : INode
{
    Node? AudioNodeRoot { get; set; }
}