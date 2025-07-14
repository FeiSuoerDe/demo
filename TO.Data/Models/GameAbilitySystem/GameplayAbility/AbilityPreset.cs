using System;
using System.Collections.Generic;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能预设
    /// </summary>
    public class AbilityPreset
    {
        public string PresetId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AbilityType Type { get; set; }
        public AbilityActivationPolicy ActivationPolicy { get; set; }
        public TimeSpan CooldownTime { get; set; }
        public HashSet<string> Tags { get; set; } = new();
        public Dictionary<string, float> Costs { get; set; } = new();
        public Dictionary<string, float> Parameters { get; set; } = new();
        public Dictionary<int, PresetLevelConfig>? LevelConfigs { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public string SoundPath { get; set; } = string.Empty;
        public string EffectPath { get; set; } = string.Empty;
        public string AnimationPath { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int MaxLevel { get; set; } = 1;
        public int UnlockLevel { get; set; } = 1;
        public HashSet<string> Prerequisites { get; set; } = new();
        public string Category { get; set; } = string.Empty;
        public AbilityRarity Rarity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
    }
    
    /// <summary>
    /// 预设等级配置
    /// </summary>
    public class PresetLevelConfig
    {
        public int Level { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
        public Dictionary<string, float> Costs { get; set; } = new();
        public TimeSpan CooldownTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public int RequiredExperience { get; set; }
        public int RequiredGold { get; set; }
        public Dictionary<string, int> RequiredMaterials { get; set; } = new();
    }
}