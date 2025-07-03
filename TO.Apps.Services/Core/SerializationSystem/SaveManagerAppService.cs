using Autofac.Features.Metadata;
using Infras.Writers;
using Infras.Writers.Abstractions;
using TO.Apps.Services.Abstractions.Bases;
using TO.Apps.Services.Abstractions.Core.SerializationSystem;
using TO.Commons.Configs;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.SerializationSystem;
using TO.Domains.Services.Abstractions.Core.AudioSystem;

namespace TO.Apps.Services.Core.SerializationSystem;

public class SaveManagerAppService : BaseService,ISaveManagerAppService
{
    private readonly IEnumerable<IDataAccess> _autoloadData;
    private readonly IEnumerable<IDataAccess> _manualLoadData;
    private readonly ISaveManagerRepo _saveManagerRepo;
    private readonly ISaveDataReaderRepo _saveDataReaderRepo;
    private readonly ISaveDataWriterRepo _saveDataWriterRepo;

    public SaveManagerAppService(IEnumerable<Meta<IDataAccess>> dataAccesses,
        ISaveManagerRepo saveManagerRepo, ISaveDataReaderRepo saveDataReaderRepo,
        ISaveDataWriterRepo saveDataWriterRepo)
    {
        var enumerable = dataAccesses.ToList();
        _autoloadData = enumerable.Where(meta => meta.Metadata["Key"].Equals(LoadType.Auto))
            .Select(meta => meta.Value);
        _manualLoadData = enumerable.Where(meta => meta.Metadata["Key"].Equals(LoadType.Manual))
            .Select(meta => meta.Value);
        _saveManagerRepo = saveManagerRepo;
        _saveDataReaderRepo = saveDataReaderRepo;
        _saveDataWriterRepo = saveDataWriterRepo;
        _saveManagerRepo.Ready += OnReady;
        
    }

    private async void OnReady()
    {
        await LoadAutosaveAsync();
    }

    public async Task LoadAutosaveAsync()
    {
        using var config = new SaveStorageConfig();
        config.CurrentPath = config.UserSettingsPath;
        config.CurrentFilename = config.UserSettingsFilename;
        var userSettings = await _saveDataReaderRepo.ReadJsonFromPathAsync<UserSettings>(config);
        foreach (var data in _autoloadData)
        {
            switch (data)
            {
                case IAudioManagerService:
                    await data.LoadAsync(userSettings?.Audio);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public async Task SaveAutosaveAsync()
    {
        using var config = new SaveStorageConfig();
        config.CurrentPath = config.UserSettingsPath;
        config.CurrentFilename = config.UserSettingsFilename;
        
        var userSettings = new UserSettings();
        foreach (var data in _autoloadData)
        {
            switch (data)
            {
                case IAudioManagerService:
                    var audio = await data.SaveAsync<UserSettings.AudioSettings>();
                    if(audio != null)
                        userSettings.Audio = audio;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        await _saveDataWriterRepo.WriteJsonToPathAsync(config,userSettings);
        
    }

    protected override void UnSubscriber()
    {
        _saveManagerRepo.Ready -= OnReady;
    }
}