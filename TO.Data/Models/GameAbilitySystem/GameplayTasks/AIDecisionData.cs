using System;
using System.Collections.Generic;
using TO.Data.Models.GameAbilitySystem.GameplayAbility;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayTasks
{
    /// <summary>
    /// AI决策历史
    /// </summary>
    public class AIDecisionHistory
    {
        public int TotalDecisions { get; set; }
        public int SuccessfulDecisions { get; set; }
        public DateTime LastDecisionTime { get; set; }
        public List<DecisionRecord> RecentDecisions { get; set; } = new();
        public Dictionary<string, AbilityPerformanceData> AbilityPerformance { get; set; } = new();
        
        public float SuccessRate => TotalDecisions > 0 ? (float)SuccessfulDecisions / TotalDecisions : 0f;
    }
    
    /// <summary>
    /// 决策记录
    /// </summary>
    public class DecisionRecord
    {
        public string AbilityId { get; set; } = string.Empty;
        public float Score { get; set; }
        public AIDecisionContext Context { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// 技能表现数据
    /// </summary>
    public class AbilityPerformanceData
    {
        public int TotalUses { get; set; }
        public int SuccessfulUses { get; set; }
        public float AverageEffectiveness { get; set; }
        
        public float SuccessRate => TotalUses > 0 ? (float)SuccessfulUses / TotalUses : 0f;
    }
    
    /// <summary>
    /// AI行为配置
    /// </summary>
    public class AIBehaviorProfile
    {
        public string ProfileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public float AggressionLevel { get; set; } = 0.5f;
        public float DefensiveLevel { get; set; } = 0.5f;
        public float SupportLevel { get; set; } = 0.5f;
        public float RiskTolerance { get; set; } = 0.5f;
        public AbilityRange PreferredRange { get; set; } = AbilityRange.Medium;
        public Dictionary<string, float> PriorityWeights { get; set; } = new();
    }
    
    /// <summary>
    /// AI决策上下文
    /// </summary>
    public class AIDecisionContext
    {
        public AttributeSet Caster { get; set; } = null!;
        public List<AttributeSet> Allies { get; set; } = new();
        public List<AttributeSet> Enemies { get; set; } = new();
        public List<TO.Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility> AvailableAbilities { get; set; } = new();
        public BattleSituationAssessment BattleSituation { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// 战斗情况评估
    /// </summary>
    public class BattleSituationAssessment
    {
        public float CasterHealthPercentage { get; set; }
        public int AllyCount { get; set; }
        public int EnemyCount { get; set; }
        public double AverageAllyHealth { get; set; }
        public double AverageEnemyHealth { get; set; }
        public float ThreatLevel { get; set; }
        public BattleSituationType SituationType { get; set; }
    }
    
    /// <summary>
    /// AI决策结果
    /// </summary>
    public class AIDecisionResult
    {
        public bool Success { get; set; }
        public string Reason { get; set; } = string.Empty;
        public TO.Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility? SelectedAbility { get; set; }
        public AttributeSet? Target { get; set; }
        public float Confidence { get; set; }
        public AIDecisionContext? DecisionContext { get; set; }
    }
    
    /// <summary>
    /// 战斗情况类型
    /// </summary>
    public enum BattleSituationType
    {
        Advantage,    // 优势
        Balanced,     // 平衡
        Disadvantage, // 劣势
        Critical      // 危急
    }
}