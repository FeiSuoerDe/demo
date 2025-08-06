using TO.Commons.Enums.System;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Trigger;

public interface IObservableSoundReactor : INode
{
    
    IObservableTrigger? Trigger { get; set; }
    
    AudioEnums.AudioType AudioType {  get; set;}
    
    AudioEnums.Audio Audio { get; set; } 
    
    float FadeDuration {  get; set;}
    
    bool Loop { get; set; }
    
    float Volume { get; set; }
}