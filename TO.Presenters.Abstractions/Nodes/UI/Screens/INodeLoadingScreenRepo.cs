using Godot;

namespace TO.Presenters.Abstractions.Nodes.UI.Screens;

public interface INodeLoadingScreenRepo
{
    ProgressBar ProgressBar { get; set; }
}