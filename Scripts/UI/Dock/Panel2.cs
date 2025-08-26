using Godot;
using System;

public partial class Panel2 : Panel
{
    // 飞船展示位置control
    [Export]
    public Control ShipDisplayPosition;

    // 缩放控制参数
    [Export]
    public float MinZoom = 0.5f;
    [Export]
    public float MaxZoom = 2.0f;
    [Export]
    public float ZoomStep = 0.1f;

    public override void _Ready()
    {
        MouseEntered += () =>
        {
            IsMouseInside = true;
        };
        MouseExited += () =>
        {
            IsMouseInside = false;
        };
    }
    // 处理输入事件
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && IsMouseInside)
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

                }
            }
        }
    }
    // 鼠标是否在区域内
    public bool IsMouseInside = false;

}
