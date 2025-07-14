using System;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能统计信息
    /// </summary>
    public class AbilityStatistics
    {
        /// <summary>
        /// 拥有者ID
        /// </summary>
        public Guid OwnerId { get; set; }
        
        /// <summary>
        /// 总技能数量
        /// </summary>
        public int TotalAbilities { get; set; }
        
        /// <summary>
        /// 准备就绪的技能数量
        /// </summary>
        public int ReadyAbilities { get; set; }
        
        /// <summary>
        /// 冷却中的技能数量
        /// </summary>
        public int OnCooldownAbilities { get; set; }
        
        /// <summary>
        /// 激活中的技能数量
        /// </summary>
        public int ActiveAbilities { get; set; }
        
        /// <summary>
        /// 禁用的技能数量
        /// </summary>
        public int DisabledAbilities { get; set; }
        
        /// <summary>
        /// 被动技能数量
        /// </summary>
        public int PassiveAbilities { get; set; }
        
        /// <summary>
        /// 平均技能等级
        /// </summary>
        public double AverageLevel { get; set; }
        
        /// <summary>
        /// 最高技能等级
        /// </summary>
        public int MaxLevel { get; set; }
        
        /// <summary>
        /// 统计生成时间
        /// </summary>
        public DateTime GeneratedTime { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public AbilityStatistics()
        {
            GeneratedTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 获取技能可用率
        /// </summary>
        /// <returns>可用技能占总技能的比例</returns>
        public double GetAvailabilityRate()
        {
            if (TotalAbilities == 0)
                return 0.0;
                
            return (double)ReadyAbilities / TotalAbilities;
        }
        
        /// <summary>
        /// 获取冷却率
        /// </summary>
        /// <returns>冷却中技能占总技能的比例</returns>
        public double GetCooldownRate()
        {
            if (TotalAbilities == 0)
                return 0.0;
                
            return (double)OnCooldownAbilities / TotalAbilities;
        }
        
        /// <summary>
        /// 获取被动技能比例
        /// </summary>
        /// <returns>被动技能占总技能的比例</returns>
        public double GetPassiveRate()
        {
            if (TotalAbilities == 0)
                return 0.0;
                
            return (double)PassiveAbilities / TotalAbilities;
        }
        
        /// <summary>
        /// 获取统计信息的字符串表示
        /// </summary>
        /// <returns>格式化的统计信息</returns>
        public override string ToString()
        {
            return $"技能统计 [拥有者: {OwnerId}] - 总数: {TotalAbilities}, 可用: {ReadyAbilities}, 冷却: {OnCooldownAbilities}, 激活: {ActiveAbilities}, 被动: {PassiveAbilities}, 平均等级: {AverageLevel:F1}";
        }
    }
}