using inFras.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Trigger;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace inFras.Nodes.UI.Trigger;

public class NodeObservableSoundReactorRepo : NodeRepo<IObservableSoundReactor>, IObservableSoundReactorRepo
{
    
    public IObservableTrigger? Trigger { get; set; }
    
    public AudioEnums.AudioType AudioType {  get; set;}
    
    public AudioEnums.Audio Audio { get; set; } 
    
    public float FadeDuration {  get; set;}
    
    public bool Loop { get; set; }
    
    public float Volume { get; set; }
    
    public NodeObservableSoundReactorRepo(IObservableSoundReactor observableSoundReactor)
    {
        Node = observableSoundReactor;
        Trigger = Node.Trigger;
        if(Trigger == null) throw new Exception("Trigger没有正确引用!");
        AudioType = Node.AudioType;
        Audio = Node.Audio;
        FadeDuration = Node.FadeDuration;
        Loop = Node.Loop;
        Volume = Node.Volume;
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
}