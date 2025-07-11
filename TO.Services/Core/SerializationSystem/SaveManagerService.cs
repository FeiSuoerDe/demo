using TO.Commons.Configs;
using TO.Data;
using TO.Data.Serialization;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Services.Abstractions.Core.SerializationSystem;
using TO.Services.Bases;

namespace TO.Services.Core.SerializationSystem;

public class SaveManagerService : BaseService,ISaveManagerService
{

    private readonly ISaveManagerRepo _saveManagerRepo;
    private readonly ISaveDataReaderRepo _saveDataReaderRepo;
    private readonly ISaveDataWriterRepo _saveDataWriterRepo;

    public SaveManagerService(
        ISaveManagerRepo saveManagerRepo, ISaveDataReaderRepo saveDataReaderRepo,
        ISaveDataWriterRepo saveDataWriterRepo)
    {
        
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
        
    }

    public async Task SaveAutosaveAsync()
    {
        using var config = new SaveStorageConfig();
        config.CurrentPath = config.UserSettingsPath;
        config.CurrentFilename = config.UserSettingsFilename;
        
        var userSettings = new UserSettings();
        

        await _saveDataWriterRepo.WriteJsonToPathAsync(config,userSettings);
        
    }

    protected override void UnSubscriber()
    {
        _saveManagerRepo.Ready -= OnReady;
    }
}