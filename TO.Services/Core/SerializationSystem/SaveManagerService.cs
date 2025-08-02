using TO.Commons.Configs;
using TO.Data.Serialization;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Services.Abstractions.Core.SerializationSystem;
using TO.Services.Bases;

namespace TO.Services.Core.SerializationSystem;

public class SaveManagerService(
    ISaveManagerRepo saveManagerRepo,
    ISaveDataReaderRepo saveDataReaderRepo,
    ISaveDataWriterRepo saveDataWriterRepo)
    : BaseService, ISaveManagerService
{
    
    public async Task<UserSettings?> LoadUserSettingsAsync()
    {
        using var config = new SaveStorageConfig();
        config.CurrentPath = config.UserSettingsPath;
        config.CurrentFilename = config.UserSettingsFilename;
        return await saveDataReaderRepo.ReadJsonFromPathAsync<UserSettings>(config);
        
    }
    

    public async Task SaveUserSettingsAsync(UserSettings userSettings)
    {
        using var config = new SaveStorageConfig();
        config.CurrentPath = config.UserSettingsPath;
        config.CurrentFilename = config.UserSettingsFilename;
        
        await saveDataWriterRepo.WriteJsonToPathAsync(config,userSettings);
        
    }

  
}