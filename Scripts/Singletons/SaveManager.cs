using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;

namespace TimelapseInvoices.Scripts.Singletons;

public partial class SaveManager : Node,ISaveManager
{
    public ILifetimeScope? NodeScope { get; set; }
    public override void _Ready()
    {
        TO.Contexts.Contexts.Instance.RegisterSingleNode<ISaveManager>(this);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 释放依赖注入容器
        NodeScope?.Dispose();
    }
}