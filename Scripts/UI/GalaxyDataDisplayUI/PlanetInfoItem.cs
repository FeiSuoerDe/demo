using Godot;
using System;

public partial class PlanetInfoItem : HBoxContainer
{
    [Export]
    public Label planetNameLabel; // 行星名称标签
    [Export]
    public Label planetTypeLabel; // 行星类型标签
    [Export]
    public Label distanceFromStarLabel; // 距离恒星的距离标签
    [Export]
    public Label volumeLabel; // 体积标签
    [Export]
    public Label massLabel; // 质量标签
    [Export]
    public Label rotationSpeedLabel; // 自转转速标签
    [Export]
    public Label revolutionPeriodLabel; // 公转周期标签

    // 设置行星信息
    public void SetPlanetInfo(string name, string type, float distance, float volume, float mass, float rotationSpeed, float revolutionPeriod)
    {
        GD.Print("设置行星信息: " + name);
        GD.Print("行星类型: " + type);
        GD.Print("距离恒星: " + distance + " AU");
        planetNameLabel.Text = name;
        planetTypeLabel.Text = type;
        distanceFromStarLabel.Text = $"距离恒星: {distance} AU";
        volumeLabel.Text = $"体积: {volume} km³";
        massLabel.Text = $"质量: {mass} kg";
        rotationSpeedLabel.Text = $"自转转速: {rotationSpeed} rad/s";
        revolutionPeriodLabel.Text = $"公转周期: {revolutionPeriod} 天";
    }
}
