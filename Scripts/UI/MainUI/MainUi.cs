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
        // 添加res://Scenes/CosmicMap/MapContShip/map_cont_ship.tscn
        var MapContShip = ResourceLoader.Load<PackedScene>(NodeController.Instance.NodeDictionary["MapContShip"]);
        var mapContShipInstance = MapContShip.Instantiate();
        if (MapContShip != null)
        {

            GetTree().Root.AddChild(mapContShipInstance);
            GD.Print("飞船控制场景已加载");
        }
        else
        {
            GD.PrintErr("无法加载飞船控制场景");
        }
        // 将相机添加到MapContShip下
        var mainCamera = ResourceLoader.Load<PackedScene>(NodeController.Instance.NodeDictionary["MainCamera"]);
        if (mainCamera != null)
        {
            var mainCameraInstance = mainCamera.Instantiate();
            mapContShipInstance.AddChild(mainCameraInstance);
            GD.Print("主相机已添加到飞船控制场景");
        }
        else
        {
            GD.PrintErr("无法加载主相机场景");
        }
    }
    private void OnExitButtonPressed()
    {
        // 处理退出按钮的逻辑
        GD.Print("退出按钮被按下");
        GetTree().Quit(); // 退出游戏
    }
}
