// 文件名: MainMenuScreen.cs
// 功能: 主菜单界面，包含开始游戏、设置和退出游戏按钮

using System;
using Contexts;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

/// <summary>
/// 主菜单界面类，继承自UIScreen基类，实现IMainMenuScreen接口
/// 用于处理游戏主菜单的显示和用户交互
/// </summary>
public partial class MainMenuScreen : Bases.UIScreen, IMainMenuScreen
{
	/// <summary>
	/// 开始游戏按钮，使用Godot的Export特性使其可在编辑器中指定
	/// </summary>
	[Export]
	public Button? StartButton { get; set; }

	/// <summary>
	/// 设置按钮，使用Godot的Export特性使其可在编辑器中指定
	/// </summary>
	[Export]
	public Button? SettingsButton { get; set; }

	/// <summary>
	/// 退出游戏按钮，使用Godot的Export特性使其可在编辑器中指定
	/// </summary>
	[Export]
	public Button? ExitButton { get; set; }

	/// <summary>
	/// 开始按钮点击事件委托，供外部订阅
	/// </summary>
	public Action? OnStartButtonPressed { get; set; }
	/// <summary>
	/// 触发开始按钮点击事件
	/// </summary>
	private void EmitStartButtonPressed() => OnStartButtonPressed?.Invoke();

	/// <summary>
	/// 设置按钮点击事件委托，供外部订阅
	/// </summary>
	public Action? OnSettingsButtonPressed { get; set; }
	/// <summary>
	/// 触发设置按钮点击事件
	/// </summary>
	private void EmitSettingsButtonPressed() => OnSettingsButtonPressed?.Invoke();

	/// <summary>
	/// 退出按钮点击事件委托，供外部订阅
	/// </summary>
	public Action? OnExitButtonPressed { get; set; }
	/// <summary>
	/// 触发退出按钮点击事件
	/// </summary>
	private void EmitExitButtonPressed() => OnExitButtonPressed?.Invoke();


	/// <summary>
	/// Godot生命周期方法，当节点进入场景树��调用
	/// 在此初始化按钮事件监听和注册节点到上下文
	/// </summary>
	public override void _Ready()
	{
		base._Ready();

		// 为按钮添加点击事件监听
		if (StartButton != null) StartButton.Pressed += EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed += EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed += EmitExitButtonPressed;
		
		// 将当前节点注册到全局节点上下文中
		NodeScope = NodeContexts.Instance.RegisterNode<IMainMenuScreen, NodeMainMenuScreenRepo>(this);
	}

	/// <summary>
	/// Godot生命周期方法，当节点退出场景树时调用
	/// 在此清理按钮事件监听，防止内存泄漏
	/// </summary>
	public override void _ExitTree()
	{
		base._ExitTree();
		
		// 移除按钮点击事件监听
		if (StartButton != null) StartButton.Pressed -= EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed -= EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed -= EmitExitButtonPressed;
	}
}