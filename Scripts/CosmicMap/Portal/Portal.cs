using Godot;
using System;

// 传送门！
public partial class Portal : Node2D
{
    public int partialId; // 传送门的唯一标识符
    [Export]
    public Area2D area; // 传送门的区域

    // area 进入时触发
    public void _on_area_2d_body_entered(Node body)
    {
        GD.Print($"传送门 {partialId} 被 {body.Name} 进入");
    }


}
