using Godot;
using System;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics;

public partial class Dock : Control
{
    // 嗨嗨这里是船坞
    // 飞船展示位置control
    [Export]
    public Control ShipDisplayPosition;
    // 当前展示飞船物理体
    [Export]
    public SpaceshipPhysics CurrentShipBody;

    // 缩放控制参数
    [Export]
    public float MinZoom = 0.5f;
    [Export]
    public float MaxZoom = 2.0f;
    [Export]
    public float ZoomStep = 0.1f;

    public override void _Ready()
    {
        // 确保ShipDisplayPosition节点已设置
        if (ShipDisplayPosition == null)
        {
            GD.PrintErr("ShipDisplayPosition节点未设置，请在编辑器中设置。");
            return;
        }

        // 初始化缩放值
        ShipDisplayPosition.Scale = Vector2.One;

        // 获取当前展示的飞船物理体
        CurrentShipBody = GetCurrentShipBody();
        if (CurrentShipBody != null)
        {
            // 如果当前飞船物理体存在，设置其展示模式
            CurrentShipBody.IsShowcaseMode = true;
            // 禁用物理模拟
            CurrentShipBody.ProcessMode = ProcessModeEnum.Disabled;
            // 添加武器槽位标记
            AddWeaponHardpointMarkings();
        }
    }

    // 处理输入事件
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp || mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                if (ShipDisplayPosition != null)
                {
                    // 获取当前缩放
                    Vector2 currentScale = ShipDisplayPosition.Scale;

                    // 根据滚轮方向调整缩放
                    if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                    {
                        // 放大
                        currentScale += new Vector2(ZoomStep, ZoomStep);
                    }
                    else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                    {
                        // 缩小
                        currentScale -= new Vector2(ZoomStep, ZoomStep);
                    }

                    // 限制缩放范围
                    currentScale.X = Mathf.Clamp(currentScale.X, MinZoom, MaxZoom);
                    currentScale.Y = Mathf.Clamp(currentScale.Y, MinZoom, MaxZoom);

                    // 应用新缩放
                    ShipDisplayPosition.Scale = currentScale;

                    // 标记事件已处理
                }
            }
        }
    }

    // 获取当前展示飞船物理体,位于ShipDisplayPosition节点下
    public SpaceshipPhysics GetCurrentShipBody()
    {
        if (ShipDisplayPosition == null)
        {
            GD.PrintErr("ShipDisplayPosition节点未设置，请检查。");
            return null;
        }

        CurrentShipBody = ShipDisplayPosition.GetNode<SpaceshipPhysics>("SpaceshipPhysics");
        if (CurrentShipBody == null)
        {
            GD.PrintErr("未找到当前展示的飞船物理体，请检查节点路径。");
        }
        return CurrentShipBody;
    }

    //给舰船每个武器槽位添加WeaponHardpointMarking
    public void AddWeaponHardpointMarkings()
    {
        if (CurrentShipBody == null) return;

        foreach (var hardpoint in CurrentShipBody.WeaponHardpoints)
        {
            var markingScene = GD.Load<PackedScene>("res://Scenes/Physics/SpaceshipPhysics/weapon/WeaponHardpointMarking.tscn");
            var markingInstance = markingScene.Instantiate<WeaponHardpointMarking>();
            hardpoint.AddChild(markingInstance);
        }
    }
}
