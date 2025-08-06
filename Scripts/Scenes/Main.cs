using Autofac;
using Godot;
using TO.Nodes.Abstractions.Scenes;
using TO.Services.Scenes;

namespace demo.Scenes;

public partial class Main : Node, IMain
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<IMain, NodeMainService>(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
}