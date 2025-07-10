
using TO.Commons.Configs;

namespace TO.Repositories.Abstractions.Core.SerializationSystem;

public interface ISaveDataReaderRepo
{
    T? ReadJsonFromPath<T>(SaveStorageConfig config) where T : class, new();
    Task<T?> ReadJsonFromPathAsync<T>(SaveStorageConfig config) where T : class, new();
}