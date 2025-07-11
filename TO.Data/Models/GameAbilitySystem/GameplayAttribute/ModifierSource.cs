using System;
using TO.Commons.Enums.Game;


namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    // SourceType 枚举已移动到 TO.Commons.Enums.GameAbilitySystemEnums
    
    /// <summary>
    /// 修饰器来源值对象
    /// </summary>
    public class ModifierSource
    {
        /// <summary>
        /// 来源ID
        /// </summary>
        public Guid SourceId { get; }
        
        /// <summary>
        /// 来源名称
        /// </summary>
        public string SourceName { get; }
        
        /// <summary>
        /// 来源类型
        /// </summary>
        public SourceType Type { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sourceId">来源ID</param>
        /// <param name="sourceName">来源名称</param>
        /// <param name="type">来源类型</param>
        public ModifierSource(Guid sourceId, string sourceName, SourceType type)
        {
            SourceId = sourceId;
            SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
            Type = type;
        }
        
        /// <summary>
        /// 判断两个修饰器来源是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is ModifierSource other)
            {
                return SourceId == other.SourceId &&
                       SourceName == other.SourceName &&
                       Type == other.Type;
            }
            return false;
        }
        
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(SourceId, SourceName, Type);
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>格式化的来源信息字符串</returns>
        public override string ToString()
        {
            return $"{SourceName} ({Type})";
        }
    }
}