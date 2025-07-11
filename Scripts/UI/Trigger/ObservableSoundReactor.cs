// 文件名: ObservableSoundReactor.cs
// 功能: 音频响应器，用于响应UI事件并播放音效，支持淡入淡出、循环等参数控制

using System;
using Autofac;
using Contexts;
using Godot;
using TO.Commons.Enums;
using TO.Commons.Enums.System;
using TO.Nodes.Abstractions.UI.Trigger;
using TO.Services.UI.Trigger;

namespace demo.UI.Trigger;

[GlobalClass]
public partial class ObservableSoundReactor : Node , IObservableSoundReactor
{
    [Export] public ObservableTrigger? TriggerNode { get; set; }
	
    public IObservableTrigger? Trigger
    {
        get => TriggerNode;
        set => TriggerNode = value as ObservableTrigger;
    }
    
    [Export] public AudioEnums.AudioType AudioType {  get; set;}
    
    [Export] public AudioEnums.Audio Audio { get; set; } 
    
    [Export] public float FadeDuration {  get; set;} = 1.0f;

    [Export] public bool Loop { get; set; } = true;
    
    [Export] public float Volume { get; set; } = 1.0f;
    
    public ILifetimeScope? NodeScope { get; set; }

    public override void _Ready()
    {
        Trigger ??= GetParent() as ObservableTrigger;
        if (Trigger == null) throw new Exception("TriggerNode is null");
        
        NodeScope = Contexts.Contexts.Instance.RegisterNode<IObservableSoundReactor, NodeObservableSoundReactorService>(this);
    }
}