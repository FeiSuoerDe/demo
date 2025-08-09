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