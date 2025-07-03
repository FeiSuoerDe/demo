using Godot;
using TO.Commons.Enums;

namespace TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Trigger;

public interface IObservableTriggerRepo
{
    Control? TriggerControl { get;  set; }
    bool TriggerOnReady { get; set; }
    
    TriggerType TriggerType { get; set; }

    void RiseTrigger(Dictionary<string, object>? data = null);
}