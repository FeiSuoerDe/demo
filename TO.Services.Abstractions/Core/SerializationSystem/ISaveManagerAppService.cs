

namespace TO.Services.Abstractions.Core.SerializationSystem;

public interface ISaveManagerAppService
{
    Task LoadAutosaveAsync();
    Task SaveAutosaveAsync();
}