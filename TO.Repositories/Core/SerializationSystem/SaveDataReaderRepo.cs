using Godot;
using GodotTask;
using Newtonsoft.Json;
using TO.Commons.Configs;
using TO.Repositories.Abstractions.Core.LogSystem;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.SerializationSystem;

public class SaveDataReaderRepo(ILoggerRepo logger) : BaseRepo,ISaveDataReaderRepo
{

    /// <summary>
    /// 从指定路径同步读取JSON文件
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="filePath">文件路径</param>
    /// <returns>反序列化的对象</returns>
    public T? ReadJsonFromPath<T>(SaveStorageConfig config) where T : class, new()
    {
        var dirPath = SaveStorageConfig.GetPath(config);
        var filePath = dirPath.PathJoin(config.CurrentFilename);

        using var dir = DirAccess.Open(dirPath);
        if (dir == null || !File.Exists(filePath))
        {
            logger.Info($"Settings file not found, using defaults: {filePath}");
            return new T();
        }

        using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            logger.Error($"Failed to open settings file: {filePath}");
            return new T();
        }
        logger.Info("Reading settings file: " + filePath);
        var json = file.GetAsText();
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 从指定路径异步读取JSON文件
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="config"></param>
    /// <returns>包含反序列化对象的GDTask</returns>
    public async Task<T?> ReadJsonFromPathAsync<T>(SaveStorageConfig config) where T : class, new()
    {
        return await GDTask.RunOnThreadPool(() => ReadJsonFromPath<T>(config));
    }
    
}
