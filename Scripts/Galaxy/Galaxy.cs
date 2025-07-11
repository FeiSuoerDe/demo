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
    // 星系id
    public int GalaxyId { get; set; }


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
                Name = "行星" + (i + 1),
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

