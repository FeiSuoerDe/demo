using Godot;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Screens;

public interface IMainMenuScreen : INode
{
	public Button? StartButton { get; protected set; }
	
	public Button? SettingsButton { get; protected set; }
	
	public Button? ExitButton { get; protected set; }
	
	Action? OnStartButtonPressed { get; set; }
	
	Action? OnSettingsButtonPressed { get; set; }
	
	Action? OnExitButtonPressed { get; set; }
}