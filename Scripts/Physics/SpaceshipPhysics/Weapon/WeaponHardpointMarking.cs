using Godot;
using System;

public partial class WeaponHardpointMarking : TextureRect
{
    // 鼠标进入时缩放为#ffffff5f,离开为ffffff
    public override void _Ready()
    {
        // 设置初始颜色为##ffffff5f
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        // 连接鼠标进入和离开事件
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        // 鼠标进入时缩放为##ffffff
        Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
    private void OnMouseExited()
    {
        // 鼠标离开时缩放为#ffffff5f

        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

}
