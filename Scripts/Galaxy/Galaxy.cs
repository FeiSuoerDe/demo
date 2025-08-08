<<<<<<< HEAD
using System;
using System.Collections.Generic;
using Godot;

// 为快速原型
namespace demo.Galaxy;

public partial class Galaxy : Node2D
{
    // 此类为一个星星系类，负责生成星球以及作为星系单位存储
    // 快速原型啊bro
    // 随机种子
    private static readonly Random random = new Random();

    private readonly string[] planetNames = new string[]
    {
        "苍穹", "星辰", "皓月", "流光", "天枢", "紫微", "云霄", "星河", "霜华", "曜石",
        "天狼", "玄武", "青龙", "白虎", "朱雀", "麒麟", "银河", "天穹", "星耀", "曙光",
        "晨曦", "暮霭", "星辉", "月影", "天际", "星域", "星空", "星环", "星云", "星海"
    };
    // 星球数组，下标为0是恒星
    public List<planet.Planet> Planets { get; private set; } = new List<planet.Planet>();
    // 星系名称
    public string Name { get; private set; }
    // 星系内的星球数量通常在7左右，数量越多生成下一个星球的概率越低，每个星球有20%概率拥有一个卫星，数量越多概率越低
    public override void _Ready()
    {
        Name = "银河系";
        GeneratePlanets();
    }
    // 生成星球
    private void GeneratePlanets()
    {
        // 生成恒星
        planet.Planet star = new planet.Planet
        {
            Name = "恒星",
            Volume = 1.0f,
            Mass = 1.0f,
            DistanceFromStar = 0.0f,
            IsClockwise = true,
            OrbitPeriod = 0.0f,
            Eccentricity = 0.0f
        };
        Planets.Add(star);

        // 生成其他星球
        int planetCount = random.Next(5, 10); // 随机生成5到10个星球
        for (int i = 0; i < planetCount; i++)
        {
            planet.Planet planet = new planet.Planet
            {
                Name = planetNames[random.Next(planetNames.Length)],
                Volume = (float)random.NextDouble() * 100, // 随机体积
                Mass = (float)random.NextDouble() * 100, // 随机质量
                DistanceFromStar = (float)random.NextDouble() * 100, // 随机距离恒星距离
                IsClockwise = random.Next(2) == 0, // 随机轨道方向
                OrbitPeriod = (float)random.NextDouble() * 10 + 1, // 随机轨道周期，1到11之间
                Eccentricity = (float)random.NextDouble() // 随机偏转，0到1之间
            };

            // 有20%的概率生成卫星
            if (random.NextDouble() < 0.4)
            {
                planet.Planet satellite = new planet.Planet
                {
                    Name = planet.Name + "卫星",
                    Volume = planet.Volume * 0.1f, // 卫星体积为母星的10%
                    Mass = planet.Mass * 0.1f, // 卫星质量为母星的10%
                    DistanceFromStar = planet.DistanceFromStar + 5, // 卫星距离恒星距离比母星大5
                    IsClockwise = planet.IsClockwise, // 卫星轨道方向与母星相同
                    OrbitPeriod = planet.OrbitPeriod * 0.5f, // 卫星轨道周期为母星的一半
                    Eccentricity = planet.Eccentricity * 0.5f // 卫星偏转为母星的一半
                };
                planet.Satellites.Add(satellite);
            }
            Planets.Add(planet);
        }
        PrintGalaxyInfo();
    }
    // 输出星系信息
    public void PrintGalaxyInfo()
    {
        GD.Print($"星系名称: {Name}");
        GD.Print($"星球数量: {Planets.Count}");
        foreach (var planet in Planets)
        {
            GD.Print($"星球名称: {planet.Name}, 体积: {planet.Volume}, 质量: {planet.Mass}, 距离恒星距离: {planet.DistanceFromStar}, 轨道方向: {(planet.IsClockwise ? "顺时针" : "逆时针")}, 轨道周期: {planet.OrbitPeriod}, 偏转: {planet.Eccentricity}");
            if (planet.Satellites.Count > 0)
            {
                GD.Print($"卫星数量: {planet.Satellites.Count}");
                foreach (var satellite in planet.Satellites)
                {
                    GD.Print($"卫星名称: {satellite.Name}, 体积: {satellite.Volume}, 质量: {satellite.Mass}, 距离恒星距离: {satellite.DistanceFromStar}, 轨道方向: {(satellite.IsClockwise ? "顺时针" : "逆时针")}, 轨道周期: {satellite.OrbitPeriod}, 偏转: {satellite.Eccentricity}");
=======
using Godot;
using System;
using System.Collections.Generic;

public partial class Galaxy : Node2D
{
    // 星系类
    // 星系包含一颗恒星和多个行星
    //1,星系有一颗恒星,恒星可以是红色,黄色,蓝色,白色,也可以是黑洞(使用枚举实现)
    // 2,星系有多个行星,行星可以是类地行星,气态巨行星,冰冻巨行星,岩石行星(使用枚举实现)
    // 3,行星可以有卫星,卫星可以是天然卫星(如月球),也可以是人工卫星(如空间站)(使用枚举实现)
    //4,行星有距离恒星距离,体积,质量,自传转速,公转周期等属性(使用类实现)

    //行星list
    public List<Planet> Planets { get; set; } = new List<Planet>();

    // 恒星对象
    public Star CentralStar { get; set; }


    //随机数对象
    private Random random = GameManager.GlobalRandom;
    // 星系id
    public int GalaxyId { get; set; }
    // 星系名称
    public string GalaxyName { get; set; }

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
        float lastDistance = 0f;
        for (int i = 0; i < planetCount; i++)
        {
            // 保证每颗行星距离都比前一颗远，步进范围可调整
            float minStep = 5f; // 最小距离步进
            float maxStep = 30f; // 最大距离步进
            float step = (float)(random.NextDouble() * (maxStep - minStep) + minStep);
            float distance = lastDistance + step;
            lastDistance = distance;

            Planet planet = new Planet
            {
                PName = GalaxyName + "-" + (i + 1),
                Type = (Planet.PlanetType)random.Next(0, 4), // 随机选择行星类型
                DistanceFromStar = distance, // 距离恒星递增
                Volume = (float)(random.NextDouble() * 1000 + 1), // 体积范围1到1001
                Mass = (float)(random.NextDouble() * 10 + 1), // 质量范围1到11
                RotationSpeed = (float)(random.NextDouble() * 1 + 10), // 自转转速范围1到101
                RevolutionPeriod = (float)(random.NextDouble() * 365 + 1) // 公转周期范围1到366
            };


            Planets.Add(planet);
        }

    }


    //依照列表,创建planet实体
    public void CreatePlanets()
    {
        foreach (var planet in Planets)
        {
            var planetScene = GD.Load<PackedScene>("res://Scenes/Galaxy/Planet/planet.tscn");
            var planetInstance = planetScene.Instantiate<Planet>();
            planetInstance.Name = planet.Name;
            planetInstance.Type = planet.Type;
            planetInstance.DistanceFromStar = planet.DistanceFromStar;
            planetInstance.Volume = planet.Volume;
            planetInstance.Mass = planet.Mass;
            planetInstance.RotationSpeed = planet.RotationSpeed;
            planetInstance.RevolutionPeriod = planet.RevolutionPeriod;

            // 添加卫星
            foreach (var satellite in planet.Satellites)
            {
                var satelliteScene = GD.Load<PackedScene>("res://Scenes/Galaxy/Planet/planet.tscn");
                var satelliteInstance = satelliteScene.Instantiate<Planet>();
                satelliteInstance.Name = satellite.Name;
                satelliteInstance.Type = satellite.Type;
                satelliteInstance.DistanceFromStar = satellite.DistanceFromStar;
                satelliteInstance.Volume = satellite.Volume;
                satelliteInstance.Mass = satellite.Mass;
                satelliteInstance.RotationSpeed = satellite.RotationSpeed;
                satelliteInstance.RevolutionPeriod = satellite.RevolutionPeriod;
                planetInstance.Satellites.Add(satelliteInstance);
            }
            AddChild(planetInstance); // 将行星实例添加到当前节点
            planetInstance.Position = new Vector2(planet.DistanceFromStar, 0); // 设置行星位置
            planetInstance.Rotation = 0; // 初始化行星自转角度
        }
    }



    //打印整个星系信息
    public void PrintGalaxyInfo()
    {
        GD.Print($"恒星名称: {CentralStar.Name}, 类型: {CentralStar.Type}, 质量: {CentralStar.Mass}, 半径: {CentralStar.Radius}, 亮度: {CentralStar.Luminosity}");
        GD.Print("行星信息:");
        foreach (var planet in Planets)
        {
            GD.Print($"行星名称: {planet.PName}, 类型: {planet.Type}, 距离恒星: {planet.DistanceFromStar}, 体积: {planet.Volume}, 质量: {planet.Mass}, 自转转速: {planet.RotationSpeed}, 公转周期: {planet.RevolutionPeriod}");
            if (planet.Satellites.Count > 0)
            {
                GD.Print("卫星信息:");
                foreach (var satellite in planet.Satellites)
                {
                    GD.Print($"卫星名称: {satellite.Name}, 类型: {satellite.Type}, 距离恒星: {satellite.DistanceFromStar}, 体积: {satellite.Volume}, 质量: {satellite.Mass}, 自转转速: {satellite.RotationSpeed}, 公转周期: {satellite.RevolutionPeriod}");
>>>>>>> dev_noFrame
                }
            }
        }
    }


<<<<<<< HEAD
}
=======
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

>>>>>>> dev_noFrame
