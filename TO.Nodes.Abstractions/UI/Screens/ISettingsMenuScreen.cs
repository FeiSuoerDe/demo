using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Screens;

public interface ISettingsMenuScreen : INode
{
    Action? OnAudioButtonPressed { get; set; }

    Action? OnVideoButtonPressed { get; set; }
    Action? OnBackButtonPressed { get; set; }
}