using Godot;
using System;
using System.Collections.Generic;

// 为快速原型
public partial class Galaxy : Node2D
{
    // 此类为一个星星系类，负责生成星球以及作为星系单位存储
    // 快速原型啊bro
    // 随机种子
    private static readonly Random random = new Random();

    string[] planetNames = new string[]
{
    "苍穹", "星辰", "皓月", "流光", "天枢", "紫微", "云霄", "星河", "霜华", "曜石",
    "天狼", "玄武", "青龙", "白虎", "朱雀", "麒麟", "银河", "天穹", "星耀", "曙光",
    "晨曦", "暮霭", "星辉", "月影", "天际", "星域", "星空", "星环", "星云", "星海"
};
    // 星球数组，下标为0是恒星
    public List<Planet> Planets { get; private set; } = new List<Planet>();
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
        Planet star = new Planet
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
            Planet planet = new Planet
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
                Planet satellite = new Planet
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
                }
            }
        }
    }


}