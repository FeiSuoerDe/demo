// 文件名: SequenceManager.cs
// 功能: 游戏状态序列管理器，负责管理和连接游戏中的不同状态

using Autofac;
using Godot;
using TO.Commons.Configs;
using TO.Nodes.Abstractions.Singletons;

namespace demo.Singletons;

/// <summary>
/// 游戏状态序列管理器
/// 负责管理和连接游戏中的不同状态
/// </summary>
public partial class SequenceManager : Node,ISequenceManager
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        var type = TO.Contexts.Contexts.Instance.GetType();
        foreach (var path in SequenceConfigs.SequencePaths)
        {
            AddChild(GD.Load<PackedScene>(path).Instantiate());
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
}