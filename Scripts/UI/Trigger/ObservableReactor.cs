// 文件名: ObservableReactor.cs
// 功能: UI事件响应器，用于响应触发器事件并执行动画效果

using System;
using Autofac;
using Contexts;
using Godot;
using Godot.Collections;
using inFras.Nodes.UI.Trigger;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace demo.UI.Trigger;

[GlobalClass]
public partial class ObservableReactor : Node, IObservableReactor
{
	[Export] public Control? ReactControl { get; set; }
	[Export] public ObservableTrigger? TriggerNode { get; set; }
	
	public IObservableTrigger? Trigger
	{
		get => TriggerNode;
		set => TriggerNode = value as ObservableTrigger;
	}

	[Export] public string FnName { get; set; } = "";
	[Export] public Array<Variant> FnArgs { get; set; } = [];
	
	[Export] public Tween.EaseType Ease { get; set; } 
	
	[Export] public Tween.TransitionType Trans { get; set; }
	
	public ILifetimeScope? NodeScope { get; set; }
	public override void _Ready()
	{
		TriggerNode ??= GetParent() as ObservableTrigger;
		if (ReactControl == null) throw new Exception("TriggerNode is null");
		ReactControl ??= GetParent() as Control;
		if (ReactControl == null) throw new Exception("TriggerControl is null");
		NodeScope = NodeContexts.Instance.RegisterNode<IObservableReactor, NodeObservableReactorRepo>(this);
	}

}
