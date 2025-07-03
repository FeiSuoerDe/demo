using Autofac.Features.Indexed;
using TO.Apps.Events;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.AudioSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Domains.Services.Abstractions.Core.AudioSystem;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeVolumeSettingsScreenService : BaseService
{
    private readonly IVolumeSettingsScreenRepo _volumeSettingsScreenRepo;
    private readonly IAudioManagerService _audioManagerService;
    private readonly IAudioManagerRepo _audioManagerRepo;
    private readonly IIndex<EventEnums, IEventBus> _eventBus;

    public NodeVolumeSettingsScreenService(IVolumeSettingsScreenRepo volumeSettingsScreenRepo,
        IAudioManagerService audioManagerService, IAudioManagerRepo audioManagerRepo, IIndex<EventEnums, IEventBus> eventBus)
    {

        _volumeSettingsScreenRepo = volumeSettingsScreenRepo;

        _audioManagerService = audioManagerService;
        _audioManagerRepo = audioManagerRepo;
        _eventBus = eventBus;

        _audioManagerRepo.OnVolumeChanged += HandleVolumeChangedDeferred;
        _audioManagerRepo.OnMuteStateChanged += _volumeSettingsScreenRepo.SetMuteState;

        _volumeSettingsScreenRepo.OnMasterVolumeChanged += SetMasterVolume;
        _volumeSettingsScreenRepo.OnMusicVolumeChanged += SetMusicVolume;
        _volumeSettingsScreenRepo.OnSFXVolumeChanged += SetSFXVolume;
        _volumeSettingsScreenRepo.OnAmbientVolumeChanged += SetAmbientVolume;
        _volumeSettingsScreenRepo.OnVoiceVolumeChanged += SetVoiceVolume;
        _volumeSettingsScreenRepo.OnMuteStateChanged += SetMuteState;
        _volumeSettingsScreenRepo.OnBackButtonPressed += OnBackButtonPressed;
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
                _volumeSettingsScreenRepo.SetMasterVolume((float)volume);
                break;
            case AudioEnums.AudioType.Music:
                _volumeSettingsScreenRepo.SetMusicVolume((float)volume);
                break;
            case AudioEnums.AudioType.SoundEffect:

                _volumeSettingsScreenRepo.SetSFXVolume((float)volume);
                break;

            case AudioEnums.AudioType.Voice:

                _volumeSettingsScreenRepo.SetVoiceVolume((float)volume);
                break;
            case AudioEnums.AudioType.Ambient:
                _volumeSettingsScreenRepo.SetAmbientVolume((float)volume);
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
        _eventBus[EventEnums.UI].Publish(new HideUI());
    }
    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _audioManagerRepo.OnVolumeChanged -= HandleVolumeChangedDeferred;
        _audioManagerRepo.OnMuteStateChanged -= _volumeSettingsScreenRepo.SetMuteState;
        _volumeSettingsScreenRepo.OnMasterVolumeChanged -= SetMasterVolume;
        _volumeSettingsScreenRepo.OnMusicVolumeChanged -= SetMusicVolume;
        _volumeSettingsScreenRepo.OnSFXVolumeChanged -= SetSFXVolume;
        _volumeSettingsScreenRepo.OnVoiceVolumeChanged -= SetVoiceVolume;
        _volumeSettingsScreenRepo.OnAmbientVolumeChanged -= SetAmbientVolume;
        _volumeSettingsScreenRepo.OnMuteStateChanged -= SetMuteState;
        _volumeSettingsScreenRepo.OnBackButtonPressed -= OnBackButtonPressed;
    }
   
}