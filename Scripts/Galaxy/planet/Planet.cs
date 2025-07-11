using System;
using System.Collections.Generic;
using Godot;

namespace demo.Galaxy.planet;

public partial class Planet : Node2D
{

    // 卫星列表
    public List<Planet> Satellites { get; private set; } = new List<Planet>();
    // Name
    public string Name { get; set; }
    // 体积
    public float Volume { get; set; }
    // 质量
    public float Mass { get; set; }
    // 距离恒星距离
    public float DistanceFromStar { get; set; }
    // 轨道方向（顺时针逆时针）
    public bool IsClockwise { get; set; }
    // 轨道周期
    public float OrbitPeriod { get; set; }
    // 偏转
    public float Eccentricity { get; set; }


    // 自转速度
    public float RotationSpeed { get; set; } = 1.0f; // 默认自转速度为1.0
    // 应用自转速度
    public override void _Process(double delta)
    {
        // 计算自转角度
        float rotationAngle = (float)(RotationSpeed * delta);
        // 应用自转
        Rotation += rotationAngle;

        // 更新卫星位置
        foreach (var satellite in Satellites)
        {
            satellite.Position = new Vector2(
                DistanceFromStar * (float)Math.Cos(satellite.Rotation),
                DistanceFromStar * (float)Math.Sin(satellite.Rotation)
            );
            satellite.Rotation += satellite.RotationSpeed * (float)delta;
        }
    }

}
