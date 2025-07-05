using System.Collections.Generic;
using Godot;

namespace TimelapseInvoices.Scripts.Autoloads;

public partial class NodeController:Node
{
    
    // 节点控制器
  
    public static NodeController Instance { get; private set; }
    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            GD.Print("加载 NodeController 实例");
        }
        else
        {
            GD.PrintErr("NodeController 实例已存在，无法创建新的实例。");
            QueueFree(); // Remove this instance if another already exists
        }
    }
    //字典
    public Dictionary<string,string> NodeDictionary { get; private set; } = new Dictionary<string, string>()
    {
        { "GameManager", "GameManager" },
        { "Galaxy", "res://Scenes/Galaxy/galaxy.tscn" },
        {"Planet", "res://Scenes/Galaxy/Planet/planet.tscn" },
        {"MainCamera","res://Scenes/Camera/main_camera_2d.tscn"}
    };
    
    
}