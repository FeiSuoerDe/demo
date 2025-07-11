using Godot;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.Singletons;

public interface ISceneManager: INode
{
    CanvasLayer? CanvasLayer { get; set; }
    ColorRect? ColorRect { get; set; }
}