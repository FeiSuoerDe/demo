using Godot;
using System;

public partial class MainCamera2d : Camera2D
{
    // 相机移动速度
    [Export]
    public float MoveSpeed { get; set; } = 500.0f;

    // 缩放速度
    [Export]
    public float ZoomSpeed { get; set; } = 0.1f;

    // 最小和最大缩放限制
    [Export]
    public float MinZoom { get; set; } = 0.1f;

    [Export]
    public float MaxZoom { get; set; } = 5.0f;

    public override void _Ready()
    {
        // 确保相机启用
        Enabled = true;
    }

    // public override void _Process(double delta)
    // {
    //     // WASD控制相机移动
    //     Vector2 inputDir = Vector2.Zero;

    //     if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
    //         inputDir.Y -= 1;
    //     if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
    //         inputDir.Y += 1;
    //     if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
    //         inputDir.X -= 1;
    //     if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
    //         inputDir.X += 1;

    //     // 如果有输入，移动相机
    //     if (inputDir != Vector2.Zero)
    //     {
    //         // 归一化向量以使对角线移动速度一致
    //         Vector2 movement = inputDir.Normalized() * MoveSpeed * (float)delta;
    //         Position += movement;
    //     }
    // }

    // public override void _Input(InputEvent @event)
    // {
    //     // 滚轮控制缩放
    //     if (@event is InputEventMouseButton mouseEvent)
    //     {
    //         if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
    //         {
    //             // 放大（缩小缩放值）
    //             Vector2 newZoom = Zoom + new Vector2(ZoomSpeed, ZoomSpeed);
    //             Zoom = new Vector2(
    //                 Mathf.Clamp(newZoom.X, MinZoom, MaxZoom),
    //                 Mathf.Clamp(newZoom.Y, MinZoom, MaxZoom)
    //             );
    //         }
    //         else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
    //         {
    //             // 缩小（增加缩放值）
    //             Vector2 newZoom = Zoom - new Vector2(ZoomSpeed, ZoomSpeed);
    //             Zoom = new Vector2(
    //                 Mathf.Clamp(newZoom.X, MinZoom, MaxZoom),
    //                 Mathf.Clamp(newZoom.Y, MinZoom, MaxZoom)
    //             );
    //         }
    //     }
    // }
}
