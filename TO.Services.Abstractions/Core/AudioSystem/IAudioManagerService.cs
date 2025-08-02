using Godot;
using TO.Commons.Enums.System;

namespace TO.Services.Abstractions.Core.AudioSystem;

public interface IAudioManagerService
{
    void PreloadAudio(string path, AudioEnums.AudioType type);
    void PlayMusic(string path, float fadeDuration = 1.0f, bool loop = true);
    AudioStreamPlayer PlaySound(string path, float volume = 1.0f);
    AudioStreamPlayer PlayVoice(string path, float volume = 1.0f);
    void SetVolume(AudioEnums.AudioType type, float volume);
    void SetMute(bool mute);
    void StopAll();
    void SetupAudioBuses();
}