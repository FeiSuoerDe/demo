using TO.Commons.Enums;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace TO.Presenters.Abstractions.Nodes.UI.Trigger;

public interface IObservableSoundReactorRepo
{
    IObservableTrigger? Trigger { get; set; }
    
    AudioEnums.AudioType AudioType {  get; set;}
    AudioEnums.Audio Audio { get; set; } 
    
    float FadeDuration {  get; set;}
    
    bool Loop { get; set; }
    
    float Volume { get; set; }
}