
using TO.Commons.Configs;

namespace TO.Repositories.Abstractions.Core.SerializationSystem;

public interface ISaveDataWriterRepo
{
    bool WriteJsonToPath<T>(SaveStorageConfig config, T settings) where T : class;

    Task<bool> WriteJsonToPathAsync<T>(SaveStorageConfig config, T json) where T : class;
}