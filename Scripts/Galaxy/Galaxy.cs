using Godot;
using System;
using System.Collections.Generic;

public partial class Galaxy : Node2D
{
 //1,星系有一颗恒星,恒星可以是红色,黄色,蓝色,白色,也可以是黑洞(使用枚举实现)
 // 2,星系有多个行星,行星可以是类地行星,气态巨行星,冰冻巨行星,岩石行星(使用枚举实现)
 // 3,行星可以有卫星,卫星可以是天然卫星(如月球),也可以是人工卫星(如空间站)(使用枚举实现)
 //4,行星有距离恒星距离,体积,质量,自传转速,公转周期等属性(使用类实现)
    
 //行星list
 public List<Planet> Planets { get; set; } = new List<Planet>();
 
 // 恒星对象
    public Star CentralStar { get; set; }
  
    
    //随机数对象
    private Random random = new Random();


    public override void _Ready()
    {
        GenerateStar();
        GeneratePlanets();
        PrintGalaxyInfo();
        
    }

    //生成恒星
    public void GenerateStar()
    {
        CentralStar = new Star
        {
            Name = "恒星" + random.Next(1, 1000),
            Type = (Star.StarType)random.Next(0, 5), // 随机选择恒星类型
            Mass = (float)(random.NextDouble() * 10 + 1), // 质量范围1到11
            Radius = (float)(random.NextDouble() * 5 + 1), // 半径范围1到6
            Luminosity = (float)(random.NextDouble() * 100 + 1) // 亮度范围1到101
        };
    }
    //生成行星,(2-15颗),每个行星有20%的概率有卫星)
    public void GeneratePlanets()
    {
        int planetCount = random.Next(2, 16); // 随机生成2到15颗行星
        for (int i = 0; i < planetCount; i++)
        {
            Planet planet = new Planet
            {
                Name = "行星" + (i + 1),
                Type = (Planet.PlanetType)random.Next(0, 4), // 随机选择行星类型
                DistanceFromStar = (float)(random.NextDouble() * 100 + 1), // 距离恒星范围1到101
                Volume = (float)(random.NextDouble() * 1000 + 1), // 体积范围1到1001
                Mass = (float)(random.NextDouble() * 10 + 1), // 质量范围1到11
                RotationSpeed = (float)(random.NextDouble() * 100 + 1), // 自转转速范围1到101
                RevolutionPeriod = (float)(random.NextDouble() * 365 + 1) // 公转周期范围1到366
            };
            
            // 有20%的概率生成卫星
            if (random.NextDouble() < 0.2)
            {
                int satelliteCount = random.Next(1, 4); // 随机生成1到3颗卫星
                for (int j = 0; j < satelliteCount; j++)
                {
                    Planet satellite = new Planet
                    {
                        Name = "卫星" + (j + 1) + " of " + planet.Name,
                        Type = (Planet.PlanetType)random.Next(0, 4), // 随机选择卫星类型
                        DistanceFromStar = planet.DistanceFromStar + (float)(random.NextDouble() * 10 + 1), // 卫星距离恒星的距离
                        Volume = (float)(random.NextDouble() * 100 + 1), // 卫星体积范围1到101
                        Mass = (float)(random.NextDouble() * 5 + 1), // 卫星质量范围1到6
                        RotationSpeed = (float)(random.NextDouble() * 50 + 1), // 卫星自转转速范围1到51
                        RevolutionPeriod = (float)(random.NextDouble() * 30 + 1) // 卫星公转周期范围1到31
                    };
                    planet.Satellites.Add(satellite);
                }
            }
            
            Planets.Add(planet);
        }
        
    }
    
    //打印整个星系信息
    public void PrintGalaxyInfo()
    {
        GD.Print($"恒星名称: {CentralStar.Name}, 类型: {CentralStar.Type}, 质量: {CentralStar.Mass}, 半径: {CentralStar.Radius}, 亮度: {CentralStar.Luminosity}");
        GD.Print("行星信息:");
        foreach (var planet in Planets)
        {
            GD.Print($"行星名称: {planet.Name}, 类型: {planet.Type}, 距离恒星: {planet.DistanceFromStar}, 体积: {planet.Volume}, 质量: {planet.Mass}, 自转转速: {planet.RotationSpeed}, 公转周期: {planet.RevolutionPeriod}");
            if (planet.Satellites.Count > 0)
            {
                GD.Print("卫星信息:");
                foreach (var satellite in planet.Satellites)
                {
                    GD.Print($"卫星名称: {satellite.Name}, 类型: {satellite.Type}, 距离恒星: {satellite.DistanceFromStar}, 体积: {satellite.Volume}, 质量: {satellite.Mass}, 自转转速: {satellite.RotationSpeed}, 公转周期: {satellite.RevolutionPeriod}");
                }
            }
        }
    }
    
    
}
// 恒星类
public class Star
{
    public string Name { get; set; } // 恒星名称
    public StarType Type { get; set; } // 恒星类型
    public float Mass { get; set; } // 质量
    public float Radius { get; set; } // 半径
    public float Luminosity { get; set; } // 亮度

    public enum StarType
    {
        RedDwarf, // 红矮星
        YellowDwarf, // 黄矮星
        BlueGiant, // 蓝巨星
        WhiteDwarf, // 白矮星
        BlackHole // 黑洞
    }
}

//行星类
public class Planet
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
}