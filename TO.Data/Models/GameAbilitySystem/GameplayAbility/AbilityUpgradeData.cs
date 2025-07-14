using System;
using System.Collections.Generic;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能升级数据
    /// </summary>
    public class AbilityUpgradeData
    {
        public string AbilityId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public int CurrentLevel { get; set; } = 0;
        public int CurrentExperience { get; set; } = 0;
        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockTime { get; set; }
        public DateTime? LastUpgradeTime { get; set; }
        public int TotalUpgrades { get; set; } = 0;
        public int TotalExperienceGained { get; set; } = 0;
        public int PlayerLevel { get; set; } = 1;
        public int AvailableGold { get; set; } = 0;
        public Dictionary<string, int> AvailableMaterials { get; set; } = new();
    }
    
    /// <summary>
    /// 升级检查结果
    /// </summary>
    public class UpgradeCheckResult
    {
        public bool CanUpgrade { get; }
        public string Reason { get; }
        
        public UpgradeCheckResult(bool canUpgrade, string reason)
        {
            CanUpgrade = canUpgrade;
            Reason = reason ?? string.Empty;
        }
    }
    
    /// <summary>
    /// 升级结果
    /// </summary>
    public class UpgradeResult
    {
        public bool Success { get; }
        public string Message { get; }
        public int NewLevel { get; }
        
        public UpgradeResult(bool success, string message, int newLevel = 0)
        {
            Success = success;
            Message = message ?? string.Empty;
            NewLevel = newLevel;
        }
    }
    
    /// <summary>
    /// 解锁结果
    /// </summary>
    public class UnlockResult
    {
        public bool Success { get; }
        public string Message { get; }
        
        public UnlockResult(bool success, string message)
        {
            Success = success;
            Message = message ?? string.Empty;
        }
    }
}