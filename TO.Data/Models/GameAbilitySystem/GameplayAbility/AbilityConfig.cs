using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能配置类
    /// 用于存储技能的配置数据，可以从配置文件或数据库中加载
    /// </summary>
    public class AbilityConfig
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        [Required]
        public string AbilityId { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能名称
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能类型
        /// </summary>
        [Required]
        public AbilityType Type { get; set; }
        
        /// <summary>
        /// 激活策略
        /// </summary>
        public AbilityActivationPolicy ActivationPolicy { get; set; } = AbilityActivationPolicy.Manual;
        
        /// <summary>
        /// 冷却时间（秒）
        /// </summary>
        [Range(0, double.MaxValue)]
        public double CooldownSeconds { get; set; }
        
        /// <summary>
        /// 技能标签
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// 技能消耗配置
        /// </summary>
        public Dictionary<string, float> Costs { get; set; } = new Dictionary<string, float>();
        
        /// <summary>
        /// 技能参数配置
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// 技能等级配置
        /// </summary>
        public List<AbilityLevelConfig> LevelConfigs { get; set; } = new List<AbilityLevelConfig>();
        
        /// <summary>
        /// 技能图标路径
        /// </summary>
        public string IconPath { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能音效路径
        /// </summary>
        public string SoundPath { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能特效路径
        /// </summary>
        public string EffectPath { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能动画路径
        /// </summary>
        public string AnimationPath { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// 最大等级
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxLevel { get; set; } = 10;
        
        /// <summary>
        /// 解锁等级
        /// </summary>
        [Range(1, int.MaxValue)]
        public int UnlockLevel { get; set; } = 1;
        
        /// <summary>
        /// 前置技能ID列表
        /// </summary>
        public List<string> PrerequisiteAbilities { get; set; } = new List<string>();
        
        /// <summary>
        /// 技能分类
        /// </summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能子分类
        /// </summary>
        public string SubCategory { get; set; } = string.Empty;
        
        /// <summary>
        /// 技能稀有度
        /// </summary>
        public string Rarity { get; set; } = "Common";
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0.0";
        
        /// <summary>
        /// 获取指定等级的配置
        /// </summary>
        /// <param name="level">技能等级</param>
        /// <returns>等级配置</returns>
        public AbilityLevelConfig? GetLevelConfig(int level)
        {
            return LevelConfigs.Find(config => config.Level == level);
        }
        
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">参数键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key">参数键</param>
        /// <param name="value">参数值</param>
        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
            UpdatedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 获取消耗值
        /// </summary>
        /// <param name="costType">消耗类型</param>
        /// <returns>消耗值</returns>
        public float GetCost(string costType)
        {
            return Costs.TryGetValue(costType, out var cost) ? cost : 0f;
        }
        
        /// <summary>
        /// 设置消耗值
        /// </summary>
        /// <param name="costType">消耗类型</param>
        /// <param name="cost">消耗值</param>
        public void SetCost(string costType, float cost)
        {
            Costs[costType] = cost;
            UpdatedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag) && !Tags.Contains(tag))
            {
                Tags.Add(tag);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveTag(string tag)
        {
            if (Tags.Remove(tag))
            {
                UpdatedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 检查是否有指定标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否有标签</returns>
        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }
        
        /// <summary>
        /// 添加前置技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        public void AddPrerequisite(string abilityId)
        {
            if (!string.IsNullOrEmpty(abilityId) && !PrerequisiteAbilities.Contains(abilityId))
            {
                PrerequisiteAbilities.Add(abilityId);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// 移除前置技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemovePrerequisite(string abilityId)
        {
            if (PrerequisiteAbilities.Remove(abilityId))
            {
                UpdatedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>验证结果</returns>
        public ValidationResult Validate()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrEmpty(AbilityId))
                errors.Add("技能ID不能为空");
                
            if (string.IsNullOrEmpty(Name))
                errors.Add("技能名称不能为空");
                
            if (CooldownSeconds < 0)
                errors.Add("冷却时间不能为负数");
                
            if (MaxLevel < 1)
                errors.Add("最大等级必须大于0");
                
            if (UnlockLevel < 1)
                errors.Add("解锁等级必须大于0");
                
            if (UnlockLevel > MaxLevel)
                errors.Add("解锁等级不能大于最大等级");
                
            // 验证等级配置
            for (int i = 0; i < LevelConfigs.Count; i++)
            {
                var levelConfig = LevelConfigs[i];
                if (levelConfig.Level < 1 || levelConfig.Level > MaxLevel)
                {
                    errors.Add($"等级配置 {i} 的等级值无效: {levelConfig.Level}");
                }
            }
            
            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        /// <returns>配置副本</returns>
        public AbilityConfig Clone()
        {
            return new AbilityConfig
            {
                AbilityId = AbilityId,
                Name = Name,
                Description = Description,
                Type = Type,
                ActivationPolicy = ActivationPolicy,
                CooldownSeconds = CooldownSeconds,
                Tags = new List<string>(Tags),
                Costs = new Dictionary<string, float>(Costs),
                Parameters = new Dictionary<string, object>(Parameters),
                LevelConfigs = LevelConfigs.ConvertAll(config => config.Clone()),
                IconPath = IconPath,
                SoundPath = SoundPath,
                EffectPath = EffectPath,
                AnimationPath = AnimationPath,
                IsEnabled = IsEnabled,
                MaxLevel = MaxLevel,
                UnlockLevel = UnlockLevel,
                PrerequisiteAbilities = new List<string>(PrerequisiteAbilities),
                Category = Category,
                SubCategory = SubCategory,
                Rarity = Rarity,
                CreatedAt = CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Version = Version
            };
        }
        
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{Name} ({AbilityId}) - {Type} - Level {UnlockLevel}-{MaxLevel}";
        }
    }
    
    /// <summary>
    /// 技能等级配置
    /// </summary>
    public class AbilityLevelConfig
    {
        /// <summary>
        /// 等级
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Level { get; set; }
        
        /// <summary>
        /// 等级参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// 等级消耗
        /// </summary>
        public Dictionary<string, float> Costs { get; set; } = new Dictionary<string, float>();
        
        /// <summary>
        /// 冷却时间（秒）
        /// </summary>
        public double? CooldownSeconds { get; set; }
        
        /// <summary>
        /// 等级描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 升级所需经验
        /// </summary>
        public int RequiredExperience { get; set; }
        
        /// <summary>
        /// 升级所需金币
        /// </summary>
        public int RequiredGold { get; set; }
        
        /// <summary>
        /// 升级所需材料
        /// </summary>
        public Dictionary<string, int> RequiredMaterials { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">参数键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key">参数键</param>
        /// <param name="value">参数值</param>
        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
        }
        
        /// <summary>
        /// 获取消耗值
        /// </summary>
        /// <param name="costType">消耗类型</param>
        /// <returns>消耗值</returns>
        public float GetCost(string costType)
        {
            return Costs.TryGetValue(costType, out var cost) ? cost : 0f;
        }
        
        /// <summary>
        /// 设置消耗值
        /// </summary>
        /// <param name="costType">消耗类型</param>
        /// <param name="cost">消耗值</param>
        public void SetCost(string costType, float cost)
        {
            Costs[costType] = cost;
        }
        
        /// <summary>
        /// 克隆等级配置
        /// </summary>
        /// <returns>配置副本</returns>
        public AbilityLevelConfig Clone()
        {
            return new AbilityLevelConfig
            {
                Level = Level,
                Parameters = new Dictionary<string, object>(Parameters),
                Costs = new Dictionary<string, float>(Costs),
                CooldownSeconds = CooldownSeconds,
                Description = Description,
                RequiredExperience = RequiredExperience,
                RequiredGold = RequiredGold,
                RequiredMaterials = new Dictionary<string, int>(RequiredMaterials)
            };
        }
        
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"Level {Level} - {Description}";
        }
    }
    
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// 错误列表
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息字符串</returns>
        public string GetErrorMessage()
        {
            return string.Join("; ", Errors);
        }
    }
}