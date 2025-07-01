

namespace TO.Apps.Services.Abstractions.Core.SerializationSystem;

public interface ISaveManagerAppService
{
    Task LoadAutosaveAsync();
    Task SaveAutosaveAsync();
}