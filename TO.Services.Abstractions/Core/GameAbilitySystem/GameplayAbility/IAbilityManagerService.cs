using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAbility;

namespace TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能管理服务接口
    /// </summary>
    public interface IAbilityManagerService
    {
        // 委托事件已移除，使用事件总线替代
        
        /// <summary>
        /// 注册技能
        /// </summary>
        /// <param name="ability">技能实例</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否成功注册</returns>
        bool RegisterAbility(Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility ability, Guid ownerId);
        
        /// <summary>
        /// 注销技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否成功注销</returns>
        bool UnregisterAbility(Guid abilityId, Guid ownerId);
        
        /// <summary>
        /// 获取技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <returns>技能实例</returns>
        Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility GetAbility(Guid abilityId);
        
        /// <summary>
        /// 获取拥有者的所有技能
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>技能列表</returns>
        List<Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility> GetOwnerAbilities(Guid ownerId);
        
        /// <summary>
        /// 根据标签查找技能
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        /// <param name="tag">标签</param>
        /// <returns>匹配的技能列表</returns>
        List<Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility> FindAbilitiesByTag(Guid ownerId, string tag);
        
        /// <summary>
        /// 根据类型查找技能
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        /// <param name="abilityType">技能类型</param>
        /// <returns>匹配的技能列表</returns>
        List<Data.Models.GameAbilitySystem.GameplayAbility.GameplayAbility> FindAbilitiesByType(Guid ownerId, AbilityType abilityType);
        
        /// <summary>
        /// 激活技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <param name="target">目标</param>
        /// <returns>激活结果</returns>
        Task<AbilityActivationResult> ActivateAbilityAsync(Guid abilityId, Guid ownerId, object target = null);
        
        /// <summary>
        /// 检查技能是否可以激活
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否可以激活</returns>
        bool CanActivateAbility(Guid abilityId, Guid ownerId);
        
        /// <summary>
        /// 取消技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否成功取消</returns>
        bool CancelAbility(Guid abilityId, Guid ownerId);
        
        /// <summary>
        /// 升级技能
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否成功升级</returns>
        bool LevelUpAbility(Guid abilityId, Guid ownerId);
        
        /// <summary>
        /// 重置技能冷却
        /// </summary>
        /// <param name="abilityId">技能ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>是否成功重置</returns>
        bool ResetAbilityCooldown(Guid abilityId, Guid ownerId);
        
        /// <summary>
        /// 更新所有技能的冷却时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        void UpdateCooldowns(float deltaTime);
        
        /// <summary>
        /// 获取拥有者的技能统计信息
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        /// <returns>技能统计信息</returns>
        AbilityStatistics GetAbilityStatistics(Guid ownerId);
        
        /// <summary>
        /// 清理拥有者的所有技能
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        void ClearOwnerAbilities(Guid ownerId);
        
        /// <summary>
        /// 获取管理器状态
        /// </summary>
        /// <returns>管理器状态信息</returns>
        string GetStatus();
    }
}