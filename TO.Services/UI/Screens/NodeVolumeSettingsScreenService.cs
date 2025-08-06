using TO.Commons.Enums.System;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Repositories.Abstractions.Core.AudioSystem;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.AudioSystem;
using TO.Services.Bases;

namespace TO.Services.UI.Screens;

public class NodeVolumeSettingsScreenService : BaseService
{
    private readonly IVolumeSettingsScreen _volumeSettingsScreen;
    private readonly IAudioManagerService _audioManagerService;
    private readonly IAudioManagerRepo _audioManagerRepo;
    private readonly IEventBusRepo _iEventBusRepo;

    public NodeVolumeSettingsScreenService(IVolumeSettingsScreen volumeSettingsScreen,
        IAudioManagerService audioManagerService, IAudioManagerRepo audioManagerRepo, IEventBusRepo iEventBusRepo)
    {

        _volumeSettingsScreen = volumeSettingsScreen;

        _audioManagerService = audioManagerService;
        _audioManagerRepo = audioManagerRepo;
        _iEventBusRepo = iEventBusRepo;

        _audioManagerRepo.OnVolumeChanged += HandleVolumeChangedDeferred;
        _audioManagerRepo.OnMuteStateChanged += _volumeSettingsScreen.SetMuteState;

        _volumeSettingsScreen.OnMasterVolumeChanged += SetMasterVolume;
        _volumeSettingsScreen.OnMusicVolumeChanged += SetMusicVolume;
        _volumeSettingsScreen.OnSFXVolumeChanged += SetSFXVolume;
        _volumeSettingsScreen.OnAmbientVolumeChanged += SetAmbientVolume;
        _volumeSettingsScreen.OnVoiceVolumeChanged += SetVoiceVolume;
        _volumeSettingsScreen.OnMuteStateChanged += SetMuteState;
        _volumeSettingsScreen.OnBackButtonPressed += OnBackButtonPressed;
        foreach (var pair in audioManagerRepo.Volumes)
        {
            HandleVolumeChangedDeferred(pair.Key, pair.Value);
        }
    }
    private void HandleVolumeChangedDeferred(AudioEnums.AudioType type, double volume)
    {
        // 原有 switch 逻辑
        switch (type)
        {

            case AudioEnums.AudioType.Master:
                _volumeSettingsScreen.SetMasterVolume((float)volume);
                break;
            case AudioEnums.AudioType.Music:
                _volumeSettingsScreen.SetMusicVolume((float)volume);
                break;
            case AudioEnums.AudioType.SoundEffect:

                _volumeSettingsScreen.SetSFXVolume((float)volume);
                break;

            case AudioEnums.AudioType.Voice:

                _volumeSettingsScreen.SetVoiceVolume((float)volume);
                break;
            case AudioEnums.AudioType.Ambient:
                _volumeSettingsScreen.SetAmbientVolume((float)volume);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    private void SetMasterVolume(double value)
    {
        _audioManagerService.SetVolume(AudioEnums.AudioType.Master, (float)value);
    }

    private void SetMusicVolume(double value)
    {
        _audioManagerService.SetVolume(AudioEnums.AudioType.Music, (float)value);
    }

    private void SetSFXVolume(double value)
    {
        _audioManagerService.SetVolume(AudioEnums.AudioType.SoundEffect, (float)value);
    }

    private void SetVoiceVolume(double value)
    {
        _audioManagerService.SetVolume(AudioEnums.AudioType.Voice, (float)value);
    }

    private void SetAmbientVolume(double value)
    {
        _audioManagerService.SetVolume(AudioEnums.AudioType.Ambient, (float)value);
    }

    private void SetMuteState(bool mute)
    {
        _audioManagerService.SetMute(mute);
    }

    private void OnBackButtonPressed()
    {
        _iEventBusRepo.Publish(new HideUI());
    }
    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _audioManagerRepo.OnVolumeChanged -= HandleVolumeChangedDeferred;
        _audioManagerRepo.OnMuteStateChanged -= _volumeSettingsScreen.SetMuteState;
        _volumeSettingsScreen.OnMasterVolumeChanged -= SetMasterVolume;
        _volumeSettingsScreen.OnMusicVolumeChanged -= SetMusicVolume;
        _volumeSettingsScreen.OnSFXVolumeChanged -= SetSFXVolume;
        _volumeSettingsScreen.OnVoiceVolumeChanged -= SetVoiceVolume;
        _volumeSettingsScreen.OnAmbientVolumeChanged -= SetAmbientVolume;
        _volumeSettingsScreen.OnMuteStateChanged -= SetMuteState;
        _volumeSettingsScreen.OnBackButtonPressed -= OnBackButtonPressed;
    }
   
}