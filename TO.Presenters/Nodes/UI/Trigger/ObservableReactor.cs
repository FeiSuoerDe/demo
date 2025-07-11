using Godot;
using Godot.Collections;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Trigger;

public class ObservableReactor : NodeRepo<IObservableReactor> , IObservableReactorRepo
{
    public Control? ReactControl { get; set; }
    public IObservableTrigger? Trigger { get; set; }
	
    public string FnName { get; set; }
    public Array<Variant> FnArgs { get; set; }
    
    public Tween.EaseType Ease { get; set; } 
    
    public Tween.TransitionType Trans { get; set; }
    
    public ObservableReactor(IObservableReactor observableReactor)
    {
        Node = observableReactor;
        ReactControl  = Node.ReactControl;
        Trigger = Node.Trigger;
        if(Trigger == null) throw new Exception("Trigger没有正确引用!");
        FnName = Node.FnName;
        FnArgs = Node.FnArgs;
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
}