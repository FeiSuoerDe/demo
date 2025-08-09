// 文件名: SettingsMenuScreen.cs
// 功能: 设置菜单界面，包含音频、视频设置和返回按钮

using System;
using Godot;
using TimelapseInvoices.Scripts.UI.Bases;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Services.UI.Screens;

namespace TimelapseInvoices.Scripts.UI.Screens;

public partial class SettingsMenuScreen : UIScreen, ISettingsMenuScreen
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
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<ISettingsMenuScreen, NodeSettingsMenuScreenService>(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (AudioButton != null) AudioButton.Pressed -= EmitAudioButtonPressed;
        if (VideoButton != null) VideoButton.Pressed -= EmitVideoButtonPressed;
        if (BackButton != null) BackButton.Pressed -= EmitBackButtonPressed;

    }
}