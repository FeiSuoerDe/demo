using Godot;
using Godot.Collections;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Trigger;

public interface IObservableReactor : INode 
{
    Control? ReactControl { get; set; }
    public IObservableTrigger? Trigger { get; set; }
	
    public string FnName { get; set; } 
    Array<Variant> FnArgs { get; set; }
    
    Tween.EaseType Ease { get; set; } 
    
	Tween.TransitionType Trans { get; set; }
}