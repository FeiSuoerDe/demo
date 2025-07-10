using Godot;
using System;

public partial class MapContShip : RigidBody2D
{
    [Export]
    public float Speed = 200.0f; // 飞船移动速度

    public override void _Ready()
    {
        // 初始化飞船位置
        Position = new Vector2(2500, 2500);
    }

    public override void _Process(double delta)
    {
        // 获取输入方向
        Vector2 inputDir = Vector2.Zero;

        if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
            inputDir.Y -= 1;
        if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
            inputDir.Y += 1;
        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
            inputDir.X -= 1;
        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
            inputDir.X += 1;

        // 如果有输入，移动飞船
        if (inputDir != Vector2.Zero)
        {
            // 归一化向量以使对角线移动速度一致
            Vector2 movement = inputDir.Normalized() * Speed * (float)delta;
            Position += movement;
        }
    }
}
