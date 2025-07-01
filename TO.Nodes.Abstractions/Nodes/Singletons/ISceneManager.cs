using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.Singletons;

public interface ISceneManager: INode
{
    CanvasLayer? CanvasLayer { get; set; }
    ColorRect? ColorRect { get; set; }
}