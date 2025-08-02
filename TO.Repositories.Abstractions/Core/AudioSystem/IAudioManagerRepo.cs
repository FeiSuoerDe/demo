using Godot;
using TO.Commons.Enums.System;
using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Bases;

namespace TO.Repositories.Abstractions.Core.AudioSystem;

public interface IAudioManagerRepo: ISingletonNodeRepo<IAudioManager>
{
    Node? AudioNodeRoot { get; set; }
    
    Action<AudioEnums.AudioType,double>? OnVolumeChanged { get; set; }
    Action<bool>? OnMuteStateChanged { get; set; }
    
    bool MuteState { get; set; }

    /// <summary>
    /// 默认音频通道配置
    /// </summary>
     Dictionary<AudioEnums.AudioType, string> DefaultBuses { get; set; } 

    /// <summary>
    /// 音频播放器池
    /// </summary>
    Dictionary<AudioEnums.AudioType, List<AudioStreamPlayer?>> AudioPlayers { get; } 

    /// <summary>
    /// 当前播放的音乐
    /// </summary>
    AudioStreamPlayer? CurrentMusic { get; set; }

    /// <summary>
    /// 音频资源缓存
    /// </summary>
    Dictionary<string, (AudioStream Resource, AudioEnums.AudioType Type)> AudioCache { get; } 

    /// <summary>
    /// 音量设置
    /// </summary>
    Dictionary<AudioEnums.AudioType, float> Volumes { get; }
}