using Godot;

namespace TO.Commons.Configs;

/// <summary>
/// 存档存储位置配置
/// 参考data_persistence_design.md规范
/// </summary>
public sealed class SaveStorageConfig : IDisposable
{
    // 新增释放标识字段
    private bool _disposed;

    /// <summary>
    /// 基础存档路径(使用Godot的user://协议)
    /// 默认: "user://saves"
    /// </summary>
    public string BaseSavePath { get; set; } = "user://saves";

    /// <summary>
    /// 存档槽目录格式
    /// {0} = 存档槽位
    /// 默认: "save_{0}"
    /// </summary>
    public string SaveSlotFormat { get; set; } = "save_{0}";
    
    /// <summary>
    /// 用户设置目录路径(相对于基础路径)
    /// 默认: "user_settings"
    /// </summary>
    public string UserSettingsPath { get; set; } = "user_settings";

    /// <summary>
    /// 用户设置文件名
    /// 默认: "settings.json"
    /// </summary>
    public string UserSettingsFilename { get; set; } = "settings.json";
    /// <summary>
    /// 主存档文件名
    /// 默认: "player_data.json"
    /// </summary>
    public string MainSaveFilename { get; set; } = "player_data.json";

    /// <summary>
    /// 元数据文件名
    /// 默认: "meta.json"
    /// </summary>
    public string MetaFilename { get; set; } = "meta.json";
    

    /// <summary>
    /// 当前操作的文件路径
    /// </summary>
    public string CurrentPath { get; set; } = string.Empty;

    /// <summary>
    /// 当前操作的文件名
    /// </summary>
    public string CurrentFilename { get; set; } = string.Empty;

    /// <summary>
    /// 版本控制配置
    /// </summary>
    public VersionControlSettings VersionControl { get; set; } = new VersionControlSettings();


    /// <summary>
    /// 版本控制设置
    /// 遵循语义化版本规范(major.minor.patch):
    /// - major: 不兼容的API修改
    /// - minor: 向下兼容的功能新增
    /// - patch: 向下兼容的问题修正
    /// </summary>
    public class VersionControlSettings
    {
        /// <summary>
        /// 当前存档版本号
        /// 格式: major.minor.patch
        /// 示例: 1.2.3 表示第1个大版本，第2个小版本，第3个补丁
        /// </summary>
        public string CurrentVersion { get; set; } = "1.0.0";

        /// <summary>
        /// 最小兼容版本号
        /// 当存档版本低于此版本时需要进行迁移
        /// 设置为"0.0.0"表示兼容所有历史版本
        /// </summary>
        public string MinCompatibleVersion { get; set; } = "1.0.0";

        /// <summary>
        /// 是否启用版本迁移
        /// 启用后会自动将旧版本存档迁移到当前版本
        /// 需要实现IVersionMigrator接口
        /// </summary>
        public bool EnableMigration { get; set; } = true;
    }
    
    public static string GetPath(SaveStorageConfig config)
    {
        // 组合完整路径: 基础路径 + 用户设置路径
        var path = config.BaseSavePath.PathJoin(config.CurrentPath);
        return ProjectSettings.GlobalizePath(path);
    }

    // 新增标准Dispose模式实现
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管资源（当前无需要释放的托管资源）
            }

            // 释放非托管资源（当前无需要释放的非托管资源）
            _disposed = true;
        }
    }

    // 新增析构函数
    ~SaveStorageConfig()
    {
        Dispose(false);
    }
}