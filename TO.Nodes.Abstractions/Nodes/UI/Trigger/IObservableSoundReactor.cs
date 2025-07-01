using Godot;
using TO.Commons.Enums;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Trigger;

public interface IObservableSoundReactor : INode
{
    
    IObservableTrigger? Trigger { get; set; }
    
    AudioEnums.AudioType AudioType {  get; set;}
    
    AudioEnums.Audio Audio { get; set; } 
    
    float FadeDuration {  get; set;}
    
    bool Loop { get; set; }
    
    float Volume { get; set; }
}