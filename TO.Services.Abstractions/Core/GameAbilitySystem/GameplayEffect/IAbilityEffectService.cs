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
        /// 应用效果到目标
        /// </summary>
        /// <param name="effect">要应用的效果</param>
        /// <param name="target">目标属性集</param>
        /// <param name="source">效果来源属性集</param>
        /// <param name="sourceAbilityId">来源技能ID</param>
        /// <returns>是否成功应用</returns>
        bool ApplyEffect(AttributeEffect effect, AttributeSet target, AttributeSet? source = null, string? sourceAbilityId = null);
        
        /// <summary>
        /// 移除指定效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <returns>是否成功移除</returns>
        bool RemoveEffect(string effectId, AttributeSet target);

        
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
        /// 刷新效果持续时间
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="target">目标属性集</param>
        /// <param name="newDuration">新的持续时间（可选）</param>
        /// <returns>是否成功刷新</returns>
        bool RefreshEffect(string effectId, AttributeSet target, TimeSpan? newDuration = null);
        
        /// <summary>
        /// 清理所有效果
        /// </summary>
        /// 清除指定目标的所有效果
        /// </summary>
        /// <param name="target">目标属性集</param>
        /// <returns>清除的效果数量</returns>
        int ClearAllEffects(AttributeSet target);
        
        /// <summary>
        /// 更新指定目标的效果（移除过期效果）
        /// </summary>
        /// <param name="target">目标属性集</param>
        /// <returns>移除的过期效果数量</returns>
        int UpdateEffects(AttributeSet target);

        int UpdatePeriodicEffects(AttributeSet target);
    }
}