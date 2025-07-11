

namespace TO.Services.Abstractions.Core.SerializationSystem;

public interface ISaveManagerService
{
    Task LoadAutosaveAsync();
    Task SaveAutosaveAsync();
}