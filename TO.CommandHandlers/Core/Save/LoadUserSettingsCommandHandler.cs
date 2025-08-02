using MediatR;
using TO.Commands.Core;
using TO.Commons.Enums.System;
using TO.Services.Abstractions.Core.AudioSystem;
using TO.Services.Abstractions.Core.SerializationSystem;

namespace Apps.Core.Save;

public class LoadUserSettingsCommandHandler(ISaveManagerService saveManagerService,
    IAudioManagerService audioManagerService) 
    : IRequestHandler<LoadUserSettingsCommand>
{
    public async Task Handle(LoadUserSettingsCommand request, CancellationToken cancellationToken)
    {
        var userSettings = await saveManagerService.LoadUserSettingsAsync();
        if (userSettings == null) return;
        
        // 应用设置到音频系统
        audioManagerService.SetVolume(AudioEnums.AudioType.Master, userSettings.Audio.MasterVolume);
        audioManagerService.SetVolume(AudioEnums.AudioType.Music, userSettings.Audio.MusicVolume);
        audioManagerService.SetVolume(AudioEnums.AudioType.SoundEffect, userSettings.Audio.SfxVolume);
        audioManagerService.SetVolume(AudioEnums.AudioType.Voice, userSettings.Audio.VoiceVolume);
        audioManagerService.SetVolume(AudioEnums.AudioType.Ambient, userSettings.Audio.AmbienceVolume);
        audioManagerService.SetMute(userSettings.Audio.Mute);


    }
}