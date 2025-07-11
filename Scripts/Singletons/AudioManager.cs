using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;

namespace demo.Singletons;

public partial class AudioManager : Node, IAudioManager
{

    /// <summary>
    /// 音频节点根节点
    /// </summary>
    public Node? AudioNodeRoot { get; set; }

    public ILifetimeScope? NodeScope { get; set; }
    

    public override void _Ready()
    {
        AudioNodeRoot = this;
        
        Contexts.Contexts.Instance.RegisterSingleNode<IAudioManager>(this);
        
    }
    

}
