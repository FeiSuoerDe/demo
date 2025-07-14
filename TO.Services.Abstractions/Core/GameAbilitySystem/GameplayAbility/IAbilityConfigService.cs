using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAbility;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能配置管理服务接口
    /// 定义技能配置加载、保存和管理的标准接口
    /// </summary>
    public interface IAbilityConfigService
    {
        /// <summary>
        /// 配置变更事件
        /// </summary>
        event Action<string, AbilityConfig>? ConfigChanged;
        
        /// <summary>
        /// 配置加载事件
        /// </summary>
        event Action<int>? ConfigsLoaded;
        
        /// <summary>
        /// 加载所有技能配置
        /// </summary>
        /// <returns>加载的配置数量</returns>
        Task<int> LoadAllConfigsAsync();
        
        /// <summary>
        /// 获取技能配置
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>技能配置</returns>
        AbilityConfig? GetConfig(string abilityId);
        
        /// <summary>
        /// 获取所有技能配置
        /// </summary>
        /// <returns>技能配置列表</returns>
        IEnumerable<AbilityConfig> GetAllConfigs();
        
        /// <summary>
        /// 根据类型获取技能配置
        /// </summary>
        /// <param name="abilityType">技能类型</param>
        /// <returns>技能配置列表</returns>
        IEnumerable<AbilityConfig> GetConfigsByType(AbilityType abilityType);
        
        /// <summary>
        /// 根据标签获取技能配置
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>技能配置列表</returns>
        IEnumerable<AbilityConfig> GetConfigsByTag(string tag);
        
        /// <summary>
        /// 根据分类获取技能配置
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns>技能配置列表</returns>
        IEnumerable<AbilityConfig> GetConfigsByCategory(string category);
        
        /// <summary>
        /// 添加或更新技能配置
        /// </summary>
        /// <param name="config">技能配置</param>
        /// <returns>是否成功</returns>
        Task<bool> AddOrUpdateConfigAsync(AbilityConfig config);
        
        /// <summary>
        /// 删除技能配置
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveConfigAsync(string abilityId);
        
        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <returns>保存任务</returns>
        Task SaveAllConfigsAsync();
        
        /// <summary>
        /// 重新加载配置
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>是否成功</returns>
        Task<bool> ReloadConfigAsync(string abilityId);
        
        /// <summary>
        /// 检查配置是否存在
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>是否存在</returns>
        bool HasConfig(string abilityId);
        
        /// <summary>
        /// 获取配置数量
        /// </summary>
        /// <returns>配置数量</returns>
        int GetConfigCount();
        
        /// <summary>
        /// 验证所有配置
        /// </summary>
        /// <returns>验证结果</returns>
        Dictionary<string, ValidationResult> ValidateAllConfigs();
        
        /// <summary>
        /// 导出配置到指定目录
        /// </summary>
        /// <param name="exportDirectory">导出目录</param>
        /// <returns>导出任务</returns>
        Task ExportConfigsAsync(string exportDirectory);
        
        /// <summary>
        /// 从指定目录导入配置
        /// </summary>
        /// <param name="importDirectory">导入目录</param>
        /// <returns>导入的配置数量</returns>
        Task<int> ImportConfigsAsync(string importDirectory);
        
        /// <summary>
        /// 清空所有配置
        /// </summary>
        void ClearAllConfigs();
        
        /// <summary>
        /// 获取配置统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        Dictionary<string, object> GetStatistics();
    }
}