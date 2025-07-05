// 文件名: SettingsMenuScreen.cs
// 功能: 设置菜单界面，包含音频、视频设置和返回按钮

using System;
using Contexts;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class SettingsMenuScreen : Bases.UIScreen, ISettingsMenuScreen
{

    [Export] public Button? AudioButton { get; set; }

    [Export] public Button? VideoButton { get; set; }

    [Export] public Button? BackButton { get; set; }

    public Action? OnAudioButtonPressed { get; set; }
    private void EmitAudioButtonPressed()  => OnAudioButtonPressed?.Invoke();
    public Action? OnVideoButtonPressed { get; set; }
     private void EmitVideoButtonPressed()  => OnVideoButtonPressed?.Invoke();
    public Action? OnBackButtonPressed { get; set; }
    private void EmitBackButtonPressed()  => OnBackButtonPressed?.Invoke();

    public override void _Ready()
    {
        base._Ready();
        if (AudioButton != null) AudioButton.Pressed += EmitAudioButtonPressed;
         if (VideoButton != null) VideoButton.Pressed += EmitVideoButtonPressed;
         if (BackButton != null) BackButton.Pressed += EmitBackButtonPressed;
        NodeScope = NodeContexts.Instance.RegisterNode<ISettingsMenuScreen, NodeSettingsMenuScreenRepo>(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (AudioButton != null) AudioButton.Pressed -= EmitAudioButtonPressed;
        if (VideoButton != null) VideoButton.Pressed -= EmitVideoButtonPressed;
        if (BackButton != null) BackButton.Pressed -= EmitBackButtonPressed;

    }
}