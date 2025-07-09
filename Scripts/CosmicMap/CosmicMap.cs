using Godot;
using System;

public partial class CosmicMap : Node2D
{

    //此乃星系地图！
    // 
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

    // 地图生成器
    public override void _Ready()
    {
        GD.Print("CosmicMap is ready.");
        GenerateCosmicMap();
        GD.Print("CosmicMap generation complete.");
        // 创建所有星系的Portal
        for (int i = 0; i < galaxyCount; i++)
        {
            Vector2 position = GetGalaxyPosition(i);
            CreatePortal(position);
        }

    }
    // 生成星系地图
    private void GenerateCosmicMap()
    {
        GD.Print("Generating cosmic map with seed: " + seed);
        Random random = new Random(seed);
        points = new Vector2[galaxyCount];

        for (int i = 0; i < galaxyCount; i++)
        {
            float x = (float)random.NextDouble() * mapSize.X;
            float y = (float)random.NextDouble() * mapSize.Y;
            points[i] = new Vector2(x, y);
            GD.Print($"Galaxy {i}: Position = {points[i]}");
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
    // 依据位置创建Portal
    public void CreatePortal(Vector2 position)
    {
        GD.Print("Creating portal at position: " + position);
        PackedScene portalScene = (PackedScene)ResourceLoader.Load("res://Scenes/CosmicMap/Portal/portal.tscn");
        if (portalScene != null)
        {
            Node2D portalInstance = (Node2D)portalScene.Instantiate();
            portalInstance.Position = position;
            AddChild(portalInstance);
            GD.Print("Portal created successfully.");
        }
        else
        {
            GD.PrintErr("Failed to load portal scene.");
        }
    }



}
