namespace TO.Commons.Enums.Game
{
    /// <summary>
    /// 技能状态枚举
    /// </summary>
    public enum AbilityStatus
    {
        /// <summary>
        /// 准备就绪
        /// </summary>
        Ready,
        
        /// <summary>
        /// 正在激活
        /// </summary>
        Activating,
        
        /// <summary>
        /// 冷却中
        /// </summary>
        OnCooldown,
        
        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled,
        
        /// <summary>
        /// 已锁定
        /// </summary>
        Locked,
        
        /// <summary>
        /// 正在引导
        /// </summary>
        Channeling,
        
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled
    }
}