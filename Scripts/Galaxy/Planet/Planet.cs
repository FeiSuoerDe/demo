using Godot;
using System;
using System.Collections.Generic;

public partial class Planet : Node2D
{
    public string Name { get; set; } // 行星名称
    public PlanetType Type { get; set; } // 行星类型
    public float DistanceFromStar { get; set; } // 距离恒星的距离
    public float Volume { get; set; } // 体积
    public float Mass { get; set; } // 质量
    public float RotationSpeed { get; set; } // 自转转速
    public float RevolutionPeriod { get; set; } // 公转周期
    // 卫星list
    public List<Planet> Satellites { get; set; } = new List<Planet>();
    public enum PlanetType
    {
        Terrestrial, // 类地行星
        GasGiant, // 气态巨行星
        IceGiant, // 冰冻巨行星
        RockyPlanet // 岩石行星
    }
    private float revolutionTime = 0f; // 用于累计公转时间

    public override void _Ready()
    {
        
        //应用距离恒星距离
        Position = new Vector2(DistanceFromStar*10 ,0); // 初始位置在X轴上，距离恒星的距离
        
    }
    
    
    //应用公转与自传
    public override void _Process(double delta)
    {
        // 累计公转时间
        revolutionTime += (float)delta;
        
        // 计算自转角度
        float rotationAngle = RotationSpeed * (float)delta;
        Rotation += rotationAngle; // 自转
        
        // 计算公转角度
        if (RevolutionPeriod > 0)
        {
            float revolutionAngle = (360f / RevolutionPeriod) * (float)revolutionTime;
            Position = new Vector2(DistanceFromStar*10, 0).Rotated(Mathf.DegToRad(revolutionAngle)); // 公转
        }
    }
}
