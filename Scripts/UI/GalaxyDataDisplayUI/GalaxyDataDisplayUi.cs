using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

public partial class GalaxyDataDisplayUi : Control
{
    [Export]
    // 关闭按钮
    public Button closeButton;
    // 星系名称
    [Export]
    public Label galaxyNameLabel;
    [Export]
    // 星球信息存储位置
    public VBoxContainer planetInfoContainer;
    // 展示星系id
    public Label galaxyIdLabel;

    public override void _Ready()
    {
        // 连接关闭按钮的信号
        closeButton.Pressed += OnCloseButtonPressed;
    }
    // 按钮关闭
    public void OnCloseButtonPressed()
    {
        // 关闭UI
        Hide();
    }
    // 查询星系数据
    public void DisplayGalaxyData(int galaxyId)
    {
        // 获取星系数据
        Galaxy galaxy = GameManager.galaxies[galaxyId];
        GD.Print("正在显示星系数据，ID: " + galaxyId);
        foreach (Planet planet in galaxy.Planets)
        {
            PackedScene packedScene = (PackedScene)ResourceLoader.Load(NodeController.Instance.NodeDictionary["PlanetInfoItem"]);
            if (packedScene != null)
            {
                // 实例化星球信息项
                PlanetInfoItem planetInfoItem = (PlanetInfoItem)packedScene.Instantiate();
                // 将星球信息项添加到容器中
                planetInfoContainer.AddChild(planetInfoItem);
                // 设置星球信息
                planetInfoItem.SetPlanetInfo(planet.PName, planet.Type.ToString(), planet.DistanceFromStar, planet.Mass, planet.RotationSpeed, planet.RevolutionPeriod, planet.Volume);
                

            }
            else
            {
                GD.PrintErr("无法加载星球信息项场景");
            }



        }

    }



}
