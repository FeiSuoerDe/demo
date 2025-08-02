using Godot;
using GodotTask;
using TO.Commons.Enums.System;
using TO.Data.Serialization;
using TO.Repositories.Abstractions.Core.AudioSystem;
using TO.Repositories.Abstractions.Core.LogSystem;
using TO.Repositories.Abstractions.Core.ResourceSystem;
using TO.Services.Abstractions.Core.AudioSystem;
using TO.Services.Bases;

namespace TO.Services.Core.AudioSystem;

public class AudioManagerService(
    IAudioManagerRepo audioManagerRepo
    ,ILoggerRepo logger
    ,IResourceLoaderRepo resourceLoader
    ) : BaseService,IAudioManagerService
{
    /// <summary>
    /// 预加载音频资源
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="type">音频类型</param>
    public void PreloadAudio(string path, AudioEnums.AudioType type)
    {
        if (audioManagerRepo.AudioCache.ContainsKey(path)) return;
        if (resourceLoader.LoadResource(path) is AudioStream audioResource)
        {
            audioManagerRepo.AudioCache[path] = (audioResource, type);
        }
        else
        {
            throw new Exception($"Failed to load audio resource: {path}");
        }
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="fadeDuration">淡入淡出时间</param>
    /// <param name="loop">是否循环</param>
    public void PlayMusic(string path, float fadeDuration = 1.0f, bool loop = true)
    {
        if (audioManagerRepo.CurrentMusic is { Playing: true })
        {
            FadeOutMusic(fadeDuration);
        }

        if (audioManagerRepo.CurrentMusic != null && audioManagerRepo.CurrentMusic.IsConnected("finished",
                Callable.From(OnMusicFinished)))
        {
            audioManagerRepo.CurrentMusic.Disconnect("finished",
                Callable.From(OnMusicFinished));
        }

        var audioResource = GetAudioResource(path);
        if (!loop && IsAudioLoop(audioResource))
        {
            logger.Fatal("Audio is imported with loop enabled, and it will always loop regardless of loop parameter!");
            
        }

        audioManagerRepo.CurrentMusic = GetAudioPlayer(AudioEnums.AudioType.Music);
        if (audioManagerRepo.CurrentMusic != null)
        {
            audioManagerRepo.CurrentMusic.Stream = audioResource;
            audioManagerRepo.CurrentMusic.Bus = audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Music];
            audioManagerRepo.CurrentMusic.VolumeDb =
                Mathf.LinearToDb(audioManagerRepo.Volumes[AudioEnums.AudioType.Music]);
            audioManagerRepo.CurrentMusic.Play();

            if (loop)
            {
                audioManagerRepo.CurrentMusic.Connect("finished",
                    Callable.From(OnMusicFinished));
            }
        }
        else
        {
            throw new Exception($"Failed to load audio resource: {path}");
        }

        if (fadeDuration > 0)
        {
            FadeInMusic(fadeDuration);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    public AudioStreamPlayer PlaySound(string path, float volume = 1.0f)
    {
        var audioResource = GetAudioResource(path);
        {
            var player = GetAudioPlayer(AudioEnums.AudioType.SoundEffect);
            player.Stream = audioResource;
            player.Bus = audioManagerRepo.DefaultBuses[AudioEnums.AudioType.SoundEffect];
            player.VolumeDb = Mathf.LinearToDb(audioManagerRepo.Volumes[AudioEnums.AudioType.SoundEffect] * volume);
            player.Play();
            return player;
        }
    }

    /// <summary>
    /// 播放语音
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    public AudioStreamPlayer PlayVoice(string path, float volume = 1.0f)
    {
        var audioResource = GetAudioResource(path);
        {
            var player = GetAudioPlayer(AudioEnums.AudioType.Voice);
            player.Stream = audioResource;
            player.Bus = audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Voice];
            player.VolumeDb = Mathf.LinearToDb(audioManagerRepo.Volumes[AudioEnums.AudioType.Voice] * volume);
            player.Play();
            return player;
        }
    }

    public void SetVolume(AudioEnums.AudioType type, float volume)
    {
        audioManagerRepo.Volumes[type] = Mathf.Clamp(volume, 0.0f, 1.0f);
        audioManagerRepo.OnVolumeChanged?.Invoke(type, audioManagerRepo.Volumes[type]);
        var busIdx = AudioServer.GetBusIndex(audioManagerRepo.DefaultBuses[type]);
        if (busIdx != -1)
        {
            AudioServer.SetBusVolumeDb(busIdx, Mathf.LinearToDb(audioManagerRepo.Volumes[type]));
        }
    }

    /// <summary>
    /// 设置全局静音状态
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        audioManagerRepo.MuteState = mute;
        audioManagerRepo.OnMuteStateChanged?.Invoke(mute);
        AudioServer.SetBusMute(AudioServer.GetBusIndex(audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Master]),
            mute);

    }

    /// <summary>
    /// 获取当前静音状态
    /// </summary>
    /// <returns>是否静音</returns>
    public bool GetMuteState() => audioManagerRepo.MuteState;

    /// <summary>
    /// 停止所有音频播放
    /// </summary>
    public void StopAll()
    {
        audioManagerRepo.CurrentMusic?.Stop();

        foreach (var player in audioManagerRepo.AudioPlayers.Values.SelectMany(players => players))
        {
            player?.Stop();
        }
    }
    
    /// <summary>
    /// 获取音频资源
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <returns>音频流资源</returns>
    private AudioStream GetAudioResource(string path)
    {
        if (audioManagerRepo.AudioCache.TryGetValue(path, out var cached))
        {
            return cached.Resource;
        }

        if (resourceLoader.LoadResource(path) is not AudioStream audioResource) throw new Exception($"Failed to load audio resource: {path}");
        audioManagerRepo.AudioCache[path] = (audioResource, AudioEnums.AudioType.SoundEffect);
        return audioResource;

    }

    /// <summary>
    /// 获取可用的音频播放器
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <returns>音频播放器</returns>
    private AudioStreamPlayer GetAudioPlayer(AudioEnums.AudioType type)
    {
        foreach (var player in audioManagerRepo.AudioPlayers[type].OfType<AudioStreamPlayer>()
                     .Where(player => !player.Playing))
        {
            return player;
        }

        var newPlayer = new AudioStreamPlayer();
        audioManagerRepo.AudioNodeRoot?.AddChild(newPlayer);
        audioManagerRepo.AudioPlayers[type].Add(newPlayer);
        return newPlayer;
    }

    /// <summary>
    /// 淡出当前音乐
    /// </summary>
    /// <param name="duration">持续时间</param>
    private void FadeOutMusic(float duration)
    {
        if (audioManagerRepo.CurrentMusic == null) return;

        var tween = audioManagerRepo.AudioNodeRoot?.CreateTween();
        tween?.TweenProperty(audioManagerRepo.CurrentMusic, "volume_db", -80.0, duration);
        tween?.TweenCallback(Callable.From(() => audioManagerRepo.CurrentMusic.Stop()));
    }

    /// <summary>
    /// 淡入当前音乐
    /// </summary>
    /// <param name="duration">持续时间</param>
    private void FadeInMusic(float duration)
    {
        if (audioManagerRepo.CurrentMusic == null) return;

        audioManagerRepo.CurrentMusic.VolumeDb = -80.0f;
        var tween = audioManagerRepo.AudioNodeRoot?.CreateTween();
        tween?.TweenProperty(audioManagerRepo.CurrentMusic, "volume_db",
            Mathf.LinearToDb(audioManagerRepo.Volumes[AudioEnums.AudioType.Music]), duration);
    }

    /// <summary>
    /// 初始化音频总线
    /// </summary>
    public void SetupAudioBuses()
    {
        // 创建主音频Bus（如果不存在）
        if (AudioServer.GetBusIndex("Master") == -1)
        {
            AudioServer.AddBus();
            var masterBusIdx = AudioServer.GetBusCount() - 1;
            AudioServer.SetBusName(masterBusIdx, "Master");
            AudioServer.SetBusVolumeDb(masterBusIdx, Mathf.LinearToDb(1.0f));
        }

        // 创建其他音频Bus
        foreach (var busName in audioManagerRepo.DefaultBuses.Select(pair => pair.Value)
                     .Where(busName => AudioServer.GetBusIndex(busName) == -1))
        { 
            AudioServer.AddBus();
            var busIdx = AudioServer.GetBusCount() - 1;
            AudioServer.SetBusName(busIdx, busName);
            AudioServer.SetBusVolumeDb(busIdx, Mathf.LinearToDb(1.0f));
            
            // 将Bus设置为Master的子Bus
            AudioServer.SetBusSend(busIdx, "Master");
        }
    }

    /// <summary>
    /// 音乐播放完毕回调
    /// </summary>
    private void OnMusicFinished()
    {
        audioManagerRepo.CurrentMusic?.Play();
    }

    /// <summary>
    /// 判断音频是否支持循环播放
    /// </summary>
    /// <param name="audio">音频资源</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>是否循环</returns>
    private bool IsAudioLoop(AudioStream? audio)
    {
        return audio switch
        {
            null => false,
            AudioStreamWav wav => wav.LoopMode != AudioStreamWav.LoopModeEnum.Disabled,
            AudioStreamMP3 or AudioStreamOggVorbis or AudioStreamPlaylist => audio._HasLoop(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #region IDataAccess Implementation
    
    public async GDTask<T?> SaveAsync<T>() where T : class
    {
        var audioSettings = new UserSettings.AudioSettings
        {
            MasterVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Master],
            MusicVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Music],
            SfxVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.SoundEffect],
            VoiceVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Voice],
            AmbienceVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Ambient],
            Mute = audioManagerRepo.MuteState
        };
        
        var result =  await GDTask.FromResult(audioSettings);
        return result as T;
    }
    
    #endregion
}