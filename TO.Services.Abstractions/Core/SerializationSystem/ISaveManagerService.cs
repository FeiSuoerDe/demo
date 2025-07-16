

using TO.Data.Serialization;

namespace TO.Services.Abstractions.Core.SerializationSystem;

public interface ISaveManagerService
{
    Task<UserSettings?> LoadUserSettingsAsync();
    Task SaveUserSettingsAsync(UserSettings userSettings);
}