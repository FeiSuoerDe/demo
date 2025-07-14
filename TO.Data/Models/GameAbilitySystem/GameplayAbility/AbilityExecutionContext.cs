using System;
using System.Collections.Generic;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayAbility
{
    /// <summary>
    /// 技能执行上下文
    /// 包含技能执行时的所有相关信息
    /// </summary>
    public class AbilityExecutionContext
    {
        /// <summary>
        /// 执行上下文ID
        /// </summary>
        public string ContextId { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 技能ID
        /// </summary>
        public string AbilityId { get; set; } = string.Empty;
        
        /// <summary>
        /// 施法者属性集
        /// </summary>
        public AttributeSet? Caster { get; set; }
        
        /// <summary>
        /// 目标对象
        /// </summary>
        public object? Target { get; set; }
        
        /// <summary>
        /// 执行开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 执行结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// 执行状态
        /// </summary>
        public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;
        
        /// <summary>
        /// 执行参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 执行结果数据
        /// </summary>
        public Dictionary<string, object> ResultData { get; set; } = new();
        
        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted => Status == ExecutionStatus.Completed || Status == ExecutionStatus.Failed;
        
        /// <summary>
        /// 执行持续时间
        /// </summary>
        public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    }
    
    /// <summary>
    /// 执行状态枚举
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Pending,
        
        /// <summary>
        /// 正在执行
        /// </summary>
        Executing,
        
        /// <summary>
        /// 执行完成
        /// </summary>
        Completed,
        
        /// <summary>
        /// 执行失败
        /// </summary>
        Failed,
        
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled
    }
}