using Godot;

namespace TimelapseInvoices.Scripts.CosmicMap;

public partial class CosmicMap : Node2D
{
    //此乃星系地图！

    [Export]
    public int seed = 0;
    // 地图范围
    [Export]
    public Vector2 mapSize = new Vector2(1000, 1000);
    // 星系数量
    [Export]
    public int galaxyCount = 10;

    // 点坐标list
    public Vector2[] points;

    private int portalCount = 0;

    // 地图生成器
    public override void _Ready()
    {
        GD.Print("CosmicMap is ready.");
        GenerateCosmicMap();
        GD.Print("CosmicMap generation complete.");
        // 读取星系名称文件使用逗号分隔,相对路径为Data\GalaxyName.txt
        // string galaxyNamesPath = @"Data\GalaxyName.txt";

        string galaxyNamesContent = System.IO.File.ReadAllText(@"Data\GalaxyName.txt");
        galaxyNames = galaxyNamesContent.Split(',');
        GD.Print("Galaxy names loaded: " + string.Join(", ", galaxyNames));



        // 创建所有星系的Portal
        for (int i = 0; i < galaxyCount; i++)
        {
            Vector2 position = GetGalaxyPosition(i);
            CreatePortal(position);
        }
    }

    // 获取星系位置
    public Vector2 GetGalaxyPosition(int index)
    {
        if (index < 0 || index >= points.Length)
        {
            GD.PrintErr("Index out of bounds: " + index);
            return Vector2.Zero;
        }
        return points[index];
    }
    // 星系名称list
    public string[] galaxyNames = new string[] { };
    // 依据位置创建Portal
    public void CreatePortal(Vector2 position)
    {
        PackedScene portalScene = (PackedScene)ResourceLoader.Load("res://Scenes/CosmicMap/Portal/portal.tscn");
        if (portalScene != null)
        {
            // 创建传送门实例
            Portal.Portal portalInstance = (Portal.Portal)portalScene.Instantiate();
            portalInstance.Position = position;
            portalInstance.partialId = portalCount;
            AddChild(portalInstance);

            // 创建星系
            Galaxy.Galaxy galaxy = new Galaxy.Galaxy();
            //    获取星系名字使用全局随机数
            if (galaxyNames.Length > 0)
            {
                int randomIndex = Autoloads.GameManager.GlobalRandom.Next(galaxyNames.Length);
                galaxy.GalaxyName = galaxyNames[randomIndex].Trim();
            }
            else
            {
                galaxy.GalaxyName = "Galaxy_" + portalCount; // 默认名称
            }
            galaxy.GalaxyId = portalCount;
            galaxy.GenerateStar();
            galaxy.GeneratePlanets();

            // 使用 Add 添加到列表末尾
            Autoloads.GameManager.galaxies.Add(galaxy);


            portalCount++;
        }
        else
        {
            GD.PrintErr("Failed to load portal scene.");
        }
    }

    // 生成星系地图
    private void GenerateCosmicMap()
    {
        GD.Print("Generating cosmic map with seed: " + seed);
        points = new Vector2[galaxyCount];

        for (int i = 0; i < galaxyCount; i++)
        {
            float x = (float)Autoloads.GameManager.GlobalRandom.NextDouble() * mapSize.X;
            float y = (float)Autoloads.GameManager.GlobalRandom.NextDouble() * mapSize.Y;
            points[i] = new Vector2(x, y);
            GD.Print($"Galaxy {i}: Position = {points[i]}");
        }
    }
}