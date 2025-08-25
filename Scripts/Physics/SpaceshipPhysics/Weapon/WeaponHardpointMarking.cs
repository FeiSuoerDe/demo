using Godot;
using System;

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
        Pressed += OnHardpointButtonPressed;
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
    // 点击事件
    private void OnHardpointButtonPressed()
    {
        ShowWeaponDetail();
        GD.Print("Weapon Hardpoint Button Pressed");
    }
    // 武器详情面板
    [Export]
    public Control WeaponDetailPanel;
    // 显示武器详情
    public void ShowWeaponDetail()
    {
        WeaponDetailPanel.Visible = !WeaponDetailPanel.Visible;
    }

}
