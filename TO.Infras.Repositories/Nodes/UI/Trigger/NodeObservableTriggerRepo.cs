using Godot;
using inFras.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Trigger;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace inFras.Nodes.UI.Trigger;

public class NodeObservableTriggerRepo : NodeRepo<IObservableTrigger> , IObservableTriggerRepo
{
    
    public Control? TriggerControl { get; set; }
    public bool TriggerOnReady { get;  set; }
    
    public TriggerType TriggerType { get; set; }
    

    public NodeObservableTriggerRepo(IObservableTrigger observableTrigger)
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