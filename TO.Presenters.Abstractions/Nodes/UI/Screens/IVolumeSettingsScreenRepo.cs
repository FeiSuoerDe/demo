namespace TO.Presenters.Abstractions.Nodes.UI.Screens;

public interface IVolumeSettingsScreenRepo
{
    Action<double>? OnMasterVolumeChanged { get; set; }
    Action<double>? OnMusicVolumeChanged { get; set; }
    Action<double>? OnSFXVolumeChanged { get; set; }
    Action<double>? OnVoiceVolumeChanged { get; set; }
    Action<double>? OnAmbientVolumeChanged { get; set; }
    
    Action<bool>? OnMuteStateChanged { get; set; }
    Action? OnBackButtonPressed { get; set; }

    void SetMasterVolume(float volume);

    void SetMusicVolume(float volume);

    void SetSFXVolume(float volume);

    void SetVoiceVolume(float volume);

    void SetAmbientVolume(float volume);
    
    void SetMuteState(bool mute);
}