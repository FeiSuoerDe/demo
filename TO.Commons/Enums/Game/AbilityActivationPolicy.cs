namespace TO.Commons.Enums.Game
{
    /// <summary>
    /// 技能激活策略枚举
    /// </summary>
    public enum AbilityActivationPolicy
    {
        /// <summary>
        /// 手动激活
        /// </summary>
        Manual,
        
        /// <summary>
        /// 自动激活
        /// </summary>
        Automatic,
        
        /// <summary>
        /// 条件触发
        /// </summary>
        Conditional,
        
        /// <summary>
        /// 事件触发
        /// </summary>
        EventTriggered,
        
        /// <summary>
        /// 被动触发
        /// </summary>
        PassiveTrigger
    }
}