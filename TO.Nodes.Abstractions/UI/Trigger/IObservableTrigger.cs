using Godot;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Trigger;

public interface IObservableTrigger : INode
{
    Control? TriggerControl { get;  set; }
    
    bool TriggerOnReady { get; set; }
    
    TriggerType TriggerType { get; set; }
    
    Action<Dictionary<string, object>?>? Triggered { get; set; }
    void RiseTrigger(Dictionary<string, object>? data = null);
}