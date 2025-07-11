using Godot;
using TO.Commons.Enums.System;
using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Core.AudioSystem;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.AudioSystem;

public class AudioManagerRepo : SingletonNodeRepo<IAudioManager>,IAudioManagerRepo
{
    public Node? AudioNodeRoot { get; set; }
    
    public Action<AudioEnums.AudioType,double>? OnVolumeChanged { get; set; }
    
    public Action<bool>? OnMuteStateChanged { get; set; }
    public bool MuteState { get; set; }
    /// <summary>
    /// 默认音频通道配置
    /// </summary>
    public Dictionary<AudioEnums.AudioType, string> DefaultBuses { get; set; } = new()
    {
        { AudioEnums.AudioType.Master, "Master"},
        { AudioEnums.AudioType.Music, "Music" },
        { AudioEnums.AudioType.SoundEffect, "SFX" },
        { AudioEnums.AudioType.Voice, "Voice" },
        { AudioEnums.AudioType.Ambient, "Ambient" }
    };

    /// <summary>
    /// 音频播放器池
    /// </summary>
    public Dictionary<AudioEnums.AudioType, List<AudioStreamPlayer?>> AudioPlayers { get; } = new(){
        { AudioEnums.AudioType.Master, []},
        { AudioEnums.AudioType.Music,  []},
        { AudioEnums.AudioType.SoundEffect, []},
        { AudioEnums.AudioType.Voice, []},
        { AudioEnums.AudioType.Ambient, []}
    };

    /// <summary>
    /// 当前播放的音乐
    /// </summary>
    public AudioStreamPlayer? CurrentMusic { get; set; }

    /// <summary>
    /// 音频资源缓存
    /// </summary>
    public Dictionary<string, (AudioStream Resource, AudioEnums.AudioType Type)> AudioCache { get; } = new();

    /// <summary>
    /// 音量设置
    /// </summary>
    public Dictionary<AudioEnums.AudioType, float> Volumes { get; } = new()
    {
        { AudioEnums.AudioType.Master, 1},
        { AudioEnums.AudioType.Music, 1 },
        { AudioEnums.AudioType.SoundEffect, 1 },
        { AudioEnums.AudioType.Voice, 1 },
        { AudioEnums.AudioType.Ambient, 1 }
    };

    protected override void Init()
    {
        base.Init();
        AudioNodeRoot = Singleton?.AudioNodeRoot;
    }
    
}