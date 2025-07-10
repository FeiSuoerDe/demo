using Godot;
using GodotTask;
using Newtonsoft.Json;
using TO.Commons.Configs;
using TO.Repositories.Abstractions.Core.LogSystem;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Repositories.Bases;
using FileAccess = Godot.FileAccess;


namespace TO.Repositories.Core.SerializationSystem;

public class SaveDataWriterRepo(ILoggerRepo logger) : BaseRepo,ISaveDataWriterRepo
{
    
    public bool WriteJsonToPath<T>(SaveStorageConfig config, T settings) where T : class
    {

        var dirPath = SaveStorageConfig.GetPath(config);
        var filePath = dirPath.PathJoin(config.CurrentFilename);

        // 确保目录存在
        DirAccess.MakeDirRecursiveAbsolute(dirPath);

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            logger.Error($"Failed to create settings directory: {dirPath}");
            return false;
        }

        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        file.StoreString(json);
        logger.Info($"Settings saved to: {filePath}");
        return  true;
    }

    public async Task<bool> WriteJsonToPathAsync<T>(SaveStorageConfig config,T json) where T : class
    {
        return await GDTask.RunOnThreadPool(() => WriteJsonToPath(config,json));
    }
    

}