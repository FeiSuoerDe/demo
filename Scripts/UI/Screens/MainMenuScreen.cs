// 文件名: MainMenuScreen.cs
// 功能: 主菜单界面，包含开始游戏、设置和退出游戏按钮

using System;
using Contexts;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class MainMenuScreen : Bases.UIScreen, IMainMenuScreen
{
	[Export]
	public Button? StartButton { get; set; }
	
	[Export]
	public Button? SettingsButton { get; set; }
	
	[Export]
	public Button? ExitButton { get; set; }
	
	public Action? OnStartButtonPressed { get; set; }
	private void EmitStartButtonPressed() => OnStartButtonPressed?.Invoke();
	
	public Action? OnSettingsButtonPressed { get; set; }
	private void EmitSettingsButtonPressed() => OnSettingsButtonPressed?.Invoke();
	
	public Action? OnExitButtonPressed { get; set; }
	private void EmitExitButtonPressed() => OnExitButtonPressed?.Invoke();
	
	
	public override void _Ready()
	{
		base._Ready();

		if (StartButton != null) StartButton.Pressed += EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed += EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed += EmitExitButtonPressed;
		NodeScope = NodeContexts.Instance.RegisterNode<IMainMenuScreen, NodeMainMenuScreenRepo>(this);
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		if (StartButton != null) StartButton.Pressed -= EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed -= EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed -= EmitExitButtonPressed;
		
	}
}