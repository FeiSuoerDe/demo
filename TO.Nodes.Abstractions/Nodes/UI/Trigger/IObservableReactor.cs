using Godot;
using Godot.Collections;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Trigger;

public interface IObservableReactor : INode 
{
    Control? ReactControl { get; set; }
    public IObservableTrigger? Trigger { get; set; }
	
    public string FnName { get; set; } 
    Array<Variant> FnArgs { get; set; }
    
    Tween.EaseType Ease { get; set; } 
    
	Tween.TransitionType Trans { get; set; }
}