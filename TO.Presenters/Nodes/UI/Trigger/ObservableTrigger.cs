using Godot;
using TO.Commons.Enums;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Trigger;

public class ObservableTrigger : NodeRepo<IObservableTrigger> , IObservableTriggerRepo
{
    
    public Control? TriggerControl { get; set; }
    public bool TriggerOnReady { get;  set; }
    
    public TriggerType TriggerType { get; set; }
    

    public ObservableTrigger(IObservableTrigger observableTrigger)
    {
        Node  = observableTrigger;
        TriggerControl = Node.TriggerControl;
        TriggerType = Node.TriggerType;
        TriggerOnReady = Node.TriggerOnReady;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
    
    
    public void RiseTrigger(Dictionary<string, object>? data = null)
    {
        Node?.Triggered?.Invoke(data);
    }
}