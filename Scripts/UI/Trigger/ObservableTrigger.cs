// 文件名: ObservableTrigger.cs
// 功能: UI触发器基类，提供通用的UI事件触发功能

using System;
using System.Collections.Generic;
using Autofac;
using Contexts;
using Godot;
using TO.Commons.Enums;
using TO.Nodes.Abstractions.UI.Trigger;
using TO.Services.UI.Trigger;

namespace demo.UI.Trigger;

[GlobalClass]
public partial class ObservableTrigger : Node,IObservableTrigger
{
	[Export] public Control? TriggerControl { get; set; }

	[Export] public bool TriggerOnReady { get; set; }
	
	[Export] public TriggerType TriggerType { get; set; }

	public Action<Dictionary<string, object>?>? Triggered { get; set; } 
	public ILifetimeScope? NodeScope { get; set; }

	public override void _Ready()
	{

		TriggerControl ??= GetParent() as Control;
		if (TriggerControl == null) throw new Exception("TriggerControl is null");
		NodeScope = Contexts.Contexts.Instance.RegisterNode<IObservableTrigger, NodeObservableTriggerService>(this);

	}
	
	public void RiseTrigger(Dictionary<string, object>? data = null)
	{
		Triggered?.Invoke(data);
	}

}
