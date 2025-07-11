using Godot;
using TO.Commons.Enums;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Abstractions.Nodes.UI.Trigger;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Trigger;

public class ObservableSoundReactor : NodeRepo<IObservableSoundReactor>, IObservableSoundReactorRepo
{
    
    public IObservableTrigger? Trigger { get; set; }
    
    public AudioEnums.AudioType AudioType {  get; set;}
    
    public AudioEnums.Audio Audio { get; set; } 
    
    public float FadeDuration {  get; set;}
    
    public bool Loop { get; set; }
    
    public float Volume { get; set; }
    
    public ObservableSoundReactor(IObservableSoundReactor observableSoundReactor)
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