// 文件名: SequenceManager.cs
// 功能: 游戏状态序列管理器，负责管理和连接游戏中的不同状态

using Godot;
using TO.Commons.Configs;

namespace demo.Singletons;

/// <summary>
/// 游戏状态序列管理器
/// 负责管理和连接游戏中的不同状态
/// </summary>
public partial class SequenceManager : Node
{
    public override void _Ready()
    {
        var type = Contexts.Contexts.Instance.GetType();
        foreach (var path in SequenceConfigs.SequencePaths)
        {
            AddChild(GD.Load<PackedScene>(path).Instantiate());
        }
       
    }
}