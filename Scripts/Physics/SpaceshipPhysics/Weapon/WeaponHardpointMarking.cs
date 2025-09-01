using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

public partial class WeaponHardpointMarking : TextureButton
{
    // 鼠标进入时缩放为#ffffff5f,离开为ffffff
    public override void _Ready()
    {
        // 设置初始颜色为##ffffff5f
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);

        // 连接鼠标进入和离开事件
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        // 连接按钮事件
    }
    private void OnMouseEntered()
    {
        // 鼠标进入时缩放为##ffffff
        Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
    private void OnMouseExited()
    {
        // 鼠标离开时缩放为#ffffff5f
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    }

    // 武器信息面板
    private Control _weaponInfoPanel;

    // 面板存放处
    [Export]
    public Control PanelContainer;

    // 显示武器详情
    public void _on_button_down()
    {
        ShowWeaponDetail();
    }

    public void ShowWeaponDetail()
    {
        GD.Print("显示武器详情面板");
        // 如果面板不存在，则从NodeController获取并实例化
        if (_weaponInfoPanel == null && PanelContainer != null)
        {
            var panelScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["WeaponInfoPanel"]);
            _weaponInfoPanel = panelScene.Instantiate<Control>();
            PanelContainer.AddChild(_weaponInfoPanel);
        }

        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = true;
        }
    }

    // 检测点击外部区域关闭详情面板
    public override void _Input(InputEvent @event)
    {
        if (_weaponInfoPanel != null && _weaponInfoPanel.Visible && @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // 获取鼠标点击位置
            Vector2 mousePosition = mouseEvent.Position;
            // 获取详情面板的位置和大小
            Rect2 panelRect = GetGlobalRect();
            // 检测点击位置是否在详情面板外部
            if (!panelRect.HasPoint(mousePosition))
            {
                _weaponInfoPanel.Visible = false;
            }
        }
    }
}
