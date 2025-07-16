using MediatR;
using TO.Commands.Core;
using TO.Commons.Enums.System;
using TO.Data.Serialization;
using TO.Repositories.Abstractions.Core.AudioSystem;
using TO.Services.Abstractions.Core.SerializationSystem;

namespace Apps.Core.Save;

public class SaveUserSettingsCommandHandler(ISaveManagerService saveManagerService,
    IAudioManagerRepo audioManagerRepo) 
    : IRequestHandler<SaveUserSettingsCommand>
{
    public async Task Handle(SaveUserSettingsCommand request, CancellationToken cancellationToken)
    {
        var userSettings = new UserSettings();
        var audioSettings = new UserSettings.AudioSettings
        {
            MasterVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Master],
            MusicVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Music],
            SfxVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.SoundEffect],
            VoiceVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Voice],
            AmbienceVolume = audioManagerRepo.Volumes[AudioEnums.AudioType.Ambient],
            Mute = audioManagerRepo.MuteState
        };
        userSettings.Audio = audioSettings;
        await saveManagerService.SaveUserSettingsAsync(userSettings);

    }
}