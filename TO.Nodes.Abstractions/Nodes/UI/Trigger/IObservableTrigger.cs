using Godot;
using TO.Commons.Enums;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Trigger;

public interface IObservableTrigger : INode
{
    Control? TriggerControl { get;  set; }
    
    bool TriggerOnReady { get; set; }
    
    TriggerType TriggerType { get; set; }
    
    Action<Dictionary<string, object>?>? Triggered { get; set; } 
}