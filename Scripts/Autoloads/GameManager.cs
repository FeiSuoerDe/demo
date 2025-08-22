using System;
using System.Collections.Generic;
using Godot;

namespace TimelapseInvoices.Scripts.Autoloads;

// 
// GameManager.cs
// 游戏管理器，负责全局状态和资源管理
public partial class GameManager : Node
{

    public static NodeController NodeController = new NodeController();
    public static GameManager Instance;
    public static List<Galaxy.Galaxy> galaxies = new List<Galaxy.Galaxy>();
    private VersionInfo versionInfo = new VersionInfo();
    // 全局随机数
    public static Random GlobalRandom = new Random(0);
    // 存储所有加载的飞船场景
    public static List<PackedScene> AllShipScenes = new List<PackedScene>();

    public override void _Ready()
    {
        // 给随机数设置种子
        if (Instance == null)
        {
            Instance = this;
            GD.Print("加载 GameManager 实例");
            // 打印版本信息
            GD.Print($"版本类型: {versionInfo.Type}, 版本号: {versionInfo.Version}");

        }
        else
        {
            GD.PrintErr("GameManager 实例已存在，无法创建新的实例。");
            QueueFree(); // Remove this instance if another already exists
        }
        // 添加 NodeController 实例到场景树
        if (NodeController != null)
        {
            GetTree().Root.CallDeferred("add_child", NodeController);
            GD.Print("NodeController 实例已添加到场景树");
        }
        else
        {
            GD.PrintErr("无法加载 NodeController 实例");
        }

        // 在游戏启动时加载所有飞船场景
        AllShipScenes = LoadAllShipScenes();
        GD.Print($"已加载 {AllShipScenes.Count} 个飞船场景");
    }

    // 读取Data\EntityStore\Vanilla\Ship路径下所有飞船物理体的场景(包括子目录)
    public static List<PackedScene> LoadAllShipScenes()
    {
        List<PackedScene> shipScenes = new List<PackedScene>();
        string basePath = "res://Data/EntityStore/Vanilla/Ship/";
        LoadShipScenesInDirectory(basePath, shipScenes);
        return shipScenes;
    }

    // 递归加载指定目录及其子目录中的所有场景
    private static void LoadShipScenesInDirectory(string dirPath, List<PackedScene> shipScenes)
    {
        var dir = DirAccess.Open(dirPath);
        if (dir != null)
        {
            // 处理当前目录下的所有场景文件
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                // 跳过 . 和 .. 目录
                if (fileName != "." && fileName != "..")
                {
                    string fullPath = dirPath + fileName;

                    if (dir.CurrentIsDir())
                    {
                        // 是目录，递归处理
                        LoadShipScenesInDirectory(fullPath + "/", shipScenes);
                    }
                    else if (fileName.EndsWith(".tscn"))
                    {
                        // 是场景文件，加载
                        var packedScene = ResourceLoader.Load<PackedScene>(fullPath);
                        if (packedScene != null)
                        {
                            shipScenes.Add(packedScene);
                            GD.Print($"加载飞船场景: {fullPath}");
                        }
                    }
                }

                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
        }
        else
        {
            GD.PrintErr($"无法打开目录: {dirPath}");
        }
    }
}

//版本信息类
public partial class VersionInfo : Node
{
    // 版本类型(测试,发布)
    public VersionType Type { get; private set; } = VersionType.Test;
    public enum VersionType
    {
        Test,
        Release
    }
    //版本号
    public string Version { get; private set; } = "1.0.0";

}