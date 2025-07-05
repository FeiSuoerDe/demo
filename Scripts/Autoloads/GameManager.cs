using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    private VersionInfo versionInfo = new VersionInfo();
    public override void _Ready()
    {
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
    