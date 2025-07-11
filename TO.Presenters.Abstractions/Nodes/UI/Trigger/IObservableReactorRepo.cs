using Godot;
using Godot.Collections;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace TO.Presenters.Abstractions.Nodes.UI.Trigger;

public interface IObservableReactorRepo
{
    Control? ReactControl { get; set; }
    IObservableTrigger? Trigger { get; set; }
	
    string FnName { get; set; }
	Array<Variant> FnArgs { get; set; }
	
	Tween.EaseType Ease { get; set; } 
    
	public Tween.TransitionType Trans { get; set; }
}