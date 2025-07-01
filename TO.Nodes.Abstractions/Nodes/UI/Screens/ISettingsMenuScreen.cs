using Godot;

using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Screens;

public interface ISettingsMenuScreen : INode
{
    Action? OnAudioButtonPressed { get; set; }

    Action? OnVideoButtonPressed { get; set; }
    Action? OnBackButtonPressed { get; set; }
}