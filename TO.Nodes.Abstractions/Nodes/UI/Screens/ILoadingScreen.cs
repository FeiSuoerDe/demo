using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Screens;

public interface ILoadingScreen : INode
{
    ProgressBar ProgressBar { get; set; }
}