using Godot;
using System;

public partial class MainUi : Control
{
    [Export]
    //开始按钮
    public Button StartButton;
    [Export]
    //退出按钮   
    public Button ExitButton;
    public override void _Ready()
    {
        // 连接按钮的信号
        StartButton.Pressed += OnStartButtonPressed;
        ExitButton.Pressed += OnExitButtonPressed;
    }
    private void OnStartButtonPressed()
    {
        // 处理开始按钮的逻辑
        GD.Print("开始按钮被按下");
        // 这里可以添加开始游戏的逻辑
    }
    private void OnExitButtonPressed()
    {
        // 处理退出按钮的逻辑
        GD.Print("退出按钮被按下");
        GetTree().Quit(); // 退出游戏
    }
}
