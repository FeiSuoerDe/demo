using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics;

public partial class Dock : Control
{
    // 嗨嗨这里是船坞
    // 左侧滚动条位置
    [Export]
    public VBoxContainer ShipScrollContainer;
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


        // 读取AllShipScenes
        if (GameManager.AllShipScenes.Count == 0)
        {
            GD.PrintErr("未加载任何飞船场景，请检查GameManager的初始化。");
            return;
        }
        else
        {
            int index = 0;
            // 通过飞船packed 中的ShipSprite来创建卡片
            foreach (var shipScene in GameManager.AllShipScenes)
            {
                // 实例化飞船场景
                var shipInstance = shipScene.Instantiate<SpaceshipPhysics>();
                if (shipInstance != null)
                {
                    // 通过NodeDictionary创建一个新的卡片
                    var shipCardScene = GD.Load<PackedScene>("res://Scenes/UI/Dock/dock_ship_card.tscn");
                    var shipCard = shipCardScene.Instantiate<DockShipCard>();

                    shipCard.ShipTexture.Texture = shipInstance.ShipSprite.Texture;
                    shipCard.DockNode = this; // 设置船坞节点
                    shipCard.index = index; // 设置索引
                    index++;

                    // 将卡片添加到滚动容器中
                    ShipScrollContainer.AddChild(shipCard);

                }
                else
                {
                    GD.PrintErr($"无法实例化飞船场景: {shipScene.ResourceName}");
                }
            }



        }


        // 连接信号
        if (ShipDetailButton != null)
        {
            ShipDetailButton.Pressed += OnShipDetailButtonPressed;
        }

        if (CloseShipDetailButton != null)
        {
            CloseShipDetailButton.Pressed += OnCloseShipDetailButtonPressed;
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
                    Vector2 currentScale = CurrentShipBody.Scale;

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
                    CurrentShipBody.Scale = currentScale;
                    GD.Print($"当前缩放: {currentScale}");

                    // 标记事件已处理
                }
            }
        }
    }
    // 设置当前展示飞船物理体
    public void SetCurrentShipBody(int index)
    {
        if (ShipDisplayPosition == null)
        {
            GD.PrintErr("ShipDisplayPosition节点未设置，请检查。");
            return;
        }

        // 清除当前展示的飞船物理体
        if (CurrentShipBody != null)
        {
            ShipDisplayPosition.RemoveChild(CurrentShipBody);
            CurrentShipBody.QueueFree();
        }

        // 获取当前展示飞船物理体
        CurrentShipBody = GameManager.AllShipScenes[index].Instantiate<SpaceshipPhysics>();
        if (CurrentShipBody != null)
        {
            ShipDisplayPosition.AddChild(CurrentShipBody);
            // 禁用物理模拟
            CurrentShipBody.ProcessMode = ProcessModeEnum.Disabled;
            // 展示模式
            CurrentShipBody.IsShowcaseMode = true;

            // 更新飞船信息
            UpdateShipInfo(CurrentShipBody.ShipData.ShipName, CurrentShipBody.ShipData.ShipModel);
            AddWeaponHardpointMarkings(); // 添加武器标记

        }
        else
        {
            GD.PrintErr($"无法实例化飞船场景: {GameManager.AllShipScenes[index].ResourceName}");
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

        // ShipDisplayPosition下只有一个节点
        CurrentShipBody = ShipDisplayPosition.GetChild<SpaceshipPhysics>(0);
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


    [Export]
    // 飞船名字
    public Label ShipNameLabel;
    [Export]
    // 型号
    public Label ShipModelLabel;

    // 更新飞船信息
    public void UpdateShipInfo(string shipName, string shipModel)
    {
        if (ShipNameLabel != null)
        {
            ShipNameLabel.Text = shipName;
        }
        if (ShipModelLabel != null)
        {
            ShipModelLabel.Text = shipModel;
        }
        richTextLabel.Text = CurrentShipBody.ShipData.GetFullInfo();
    }
    // 详细参数按钮
    [Export]
    public Button ShipDetailButton;
    [Export]
    // 详细参数面板
    public CanvasLayer ShipDetailPanel;
    // 显示详细参数面板
    public void ShowShipDetailPanel()
    {
        if (ShipDetailPanel != null)
        {
            ShipDetailPanel.Visible = true;
        }
    }
    [Export]
    // 详细信息文本text
    public RichTextLabel richTextLabel;
    // 隐藏详细参数面板
    public void HideShipDetailPanel()
    {
        if (ShipDetailPanel != null)
        {
            ShipDetailPanel.Visible = false;
        }
    }
    // 点击详细参数按钮时触发
    public void OnShipDetailButtonPressed()
    {
        if (ShipDetailPanel != null)
        {
            ShipDetailPanel.Visible = !ShipDetailPanel.Visible;
        }

    }
    [Export]
    // 关闭详细参数面板按钮
    public Button CloseShipDetailButton;
    // 点击关闭按钮时触发
    public void OnCloseShipDetailButtonPressed()
    {
        HideShipDetailPanel();
    }

}
