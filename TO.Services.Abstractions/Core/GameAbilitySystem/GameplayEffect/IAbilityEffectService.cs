using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayEffect
{
    /// <summary>
    /// 技能效果管理服务接口
    /// 定义技能效果系统的标准接口
    /// </summary>
    public interface IAbilityEffectService
    {
        /// <summary>
        /// 效果应用事件
        /// </summary>
        event Action<string, AttributeEffect, AttributeSet>? EffectApplied;
        
        /// <summary>
        /// 效果移除事件
        /// </summary>
        event Action<string, AttributeEffect, AttributeSet>? EffectRemoved;
        
        /// <summary>
        /// 效果刷新事件
        /// </summary>
        event Action<string, AttributeEffect, AttributeSet>? EffectRefreshed;
        
        /// <summary>
        /// 效果过期事件
        /// </summary>
        event Action<string, AttributeEffect, AttributeSet>? EffectExpired;
        
        /// <summary>
        /// 应用效果到目标
        /// </summary>
        /// <param name="effect">要应用的效果</param>
        /// <param name="target">目标属性集</param>
        /// <param name="source">效果来源属性集</param>
        /// <param name="sourceAbilityId">来源技能ID</param>
        /// <returns>是否成功应用</returns>
        Task<bool> ApplyEffectAsync(AttributeEffect effect, AttributeSet target, 
            AttributeSet? source = null, string? sourceAbilityId = null);
        
        /// <summary>
        /// 移除指定效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>是否成功移除</returns>
        bool RemoveEffect(string effectId, AttributeSet target);
        
        /// <summary>
        /// 移除指定类型的所有效果
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <param name="target">目标属性集</param>
        /// <returns>移除的效果数量</returns>
        int RemoveEffectsByType(EffectType effectType, AttributeSet target);
        
        /// <summary>
        /// 移除指定标签的所有效果
        /// </summary>
        /// <param name="tag">效果标签</param>
        /// <param name="target">目标属性集</param>
        /// <returns>移除的效果数量</returns>
        int RemoveEffectsByTag(string tag, AttributeSet target);
        
        /// <summary>
        /// 获取目标身上的活跃效果
        /// </summary>
        /// <param name="target">目标属性集</param>
        /// <returns>活跃效果列表</returns>
        IEnumerable<AttributeEffect> GetActiveEffects(AttributeSet target);
        
        /// <summary>
        /// 检查目标是否有指定效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>是否有该效果</returns>
        bool HasEffect(string effectId, AttributeSet target);
        
        /// <summary>
        /// 获取效果的剩余时间
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>剩余时间</returns>
        TimeSpan GetEffectRemainingTime(string effectId, AttributeSet target);
        
        /// <summary>
        /// 刷新效果持续时间
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <param name="newDuration">新的持续时间（可选）</param>
        /// <returns>是否成功刷新</returns>
        bool RefreshEffect(string effectId, AttributeSet target, TimeSpan? newDuration = null);
        
        /// <summary>
        /// 设置免疫
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="effectId">效果ID</param>
        /// <param name="duration">免疫持续时间</param>
        void SetImmunity(string targetId, string effectId, TimeSpan duration);
        
        /// <summary>
        /// 检查是否免疫
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>是否免疫</returns>
        bool IsImmune(string targetId, string effectId);
        
        /// <summary>
        /// 清理过期效果
        /// </summary>
        /// <returns>清理的效果数量</returns>
        int CleanupExpiredEffects();
        
        /// <summary>
        /// 获取效果统计信息
        /// </summary>
        /// <returns>统计信息字典</returns>
        Dictionary<string, object> GetEffectStatistics();
        
        /// <summary>
        /// 清理所有效果
        /// </summary>
        void ClearAllEffects();
    }
}