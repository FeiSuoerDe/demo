using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

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
        //添加宇宙地图
        var CosmicMap = ResourceLoader.Load<PackedScene>(NodeController.Instance.NodeDictionary["CosmicMap"]);
        //添加相机
        var cameraScene = ResourceLoader.Load<PackedScene>(NodeController.Instance.NodeDictionary["MainCamera"]);
        if (cameraScene != null)
        {
            var cameraInstance = cameraScene.Instantiate();
            GetTree().Root.AddChild(cameraInstance);
            GD.Print("相机场景已加载");
        }
        else
        {
            GD.PrintErr("无法加载相机场景");
        }
        if (CosmicMap != null)
        {
            var galaxyInstance = CosmicMap.Instantiate();
            GetTree().Root.AddChild(galaxyInstance);
            GD.Print("星系场景已加载");
            //删除自身
            QueueFree(); // 删除当前的 MainUi 实例
        }
        else
        {
            GD.PrintErr("无法加载星系场景");
        }
    }
    private void OnExitButtonPressed()
    {
        // 处理退出按钮的逻辑
        GD.Print("退出按钮被按下");
        GetTree().Quit(); // 退出游戏
    }
}
