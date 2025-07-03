using Godot;

namespace TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

public interface INodeLoadingScreenRepo
{
    ProgressBar ProgressBar { get; set; }
}