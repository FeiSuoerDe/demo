using System;
using System.Collections.Generic;
using System.Linq;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 属性计算引擎
    /// 负责处理属性修饰器的应用和计算逻辑
    /// </summary>
    public class AttributeCalculationEngine
    {
        private static readonly Lazy<AttributeCalculationEngine> _instance = 
            new Lazy<AttributeCalculationEngine>(() => new AttributeCalculationEngine());
        
        /// <summary>
        /// 单例实例
        /// </summary>
        public static AttributeCalculationEngine Instance => _instance.Value;
        
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private AttributeCalculationEngine()
        {
        }
        
        /// <summary>
        /// 计算属性的最终值
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <param name="modifiers">修饰器列表</param>
        /// <returns>计算后的最终值</returns>
        public float CalculateAttributeValue(float baseValue, IEnumerable<AttributeModifier> modifiers)
        {
            if (modifiers == null)
                return baseValue;
            
            var sortedModifiers = SortModifiersByPriority(modifiers.Where(m => !m.IsExpired));
            return ApplyModifiers(baseValue, sortedModifiers);
        }
        
        /// <summary>
        /// 按优先级排序修饰器
        /// </summary>
        /// <param name="modifiers">修饰器列表</param>
        /// <returns>排序后的修饰器列表</returns>
        private IEnumerable<AttributeModifier> SortModifiersByPriority(IEnumerable<AttributeModifier> modifiers)
        {
            // 按操作类型和执行顺序排序
            // 执行顺序：Add -> Multiply -> Percentage -> Override
            return modifiers.OrderBy(m => GetOperationPriority(m.OperationType))
                           .ThenBy(m => m.ExecutionOrder)
                           .ThenBy(m => m.Id); // 确保稳定排序
        }
        
        /// <summary>
        /// 获取操作类型的优先级
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <returns>优先级值（越小优先级越高）</returns>
        private int GetOperationPriority(ModifierOperationType operationType)
        {
            return operationType switch
            {
                ModifierOperationType.Add => 1,
                ModifierOperationType.Multiply => 2,
                ModifierOperationType.Percentage => 3,
                ModifierOperationType.Override => 4,
                _ => int.MaxValue
            };
        }
        
        /// <summary>
        /// 应用修饰器到基础值
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <param name="sortedModifiers">排序后的修饰器列表</param>
        /// <returns>应用修饰器后的值</returns>
        private float ApplyModifiers(float baseValue, IEnumerable<AttributeModifier> sortedModifiers)
        {
            float currentValue = baseValue;
            float additionSum = 0f;
            float multiplicationProduct = 1f;
            float percentageSum = 0f;
            float? overrideValue = null;
            
            foreach (var modifier in sortedModifiers)
            {
                switch (modifier.OperationType)
                {
                    case ModifierOperationType.Add:
                        additionSum += modifier.Value;
                        break;
                        
                    case ModifierOperationType.Multiply:
                        multiplicationProduct *= modifier.Value;
                        break;
                        
                    case ModifierOperationType.Percentage:
                        percentageSum += modifier.Value;
                        break;
                        
                    case ModifierOperationType.Override:
                        overrideValue = modifier.Value;
                        break;
                }
            }
            
            // 如果有覆盖值，直接返回最后一个覆盖值
            if (overrideValue.HasValue)
            {
                return overrideValue.Value;
            }
            
            // 按顺序应用修饰器：(基础值 + 加法) * 乘法 * (1 + 百分比)
            currentValue = (currentValue + additionSum) * multiplicationProduct * (1f + percentageSum / 100f);
            
            return Math.Max(0, currentValue); // 确保结果不为负数
        }
        
        /// <summary>
        /// 计算修饰器对属性值的贡献
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <param name="modifier">修饰器</param>
        /// <param name="allModifiers">所有修饰器（用于上下文计算）</param>
        /// <returns>修饰器的贡献值</returns>
        public float CalculateModifierContribution(float baseValue, AttributeModifier modifier, 
                                                  IEnumerable<AttributeModifier> allModifiers)
        {
            if (modifier.IsExpired)
                return 0f;
            
            var withoutModifier = CalculateAttributeValue(baseValue, 
                allModifiers.Where(m => !m.Id.Equals(modifier.Id)));
            var withModifier = CalculateAttributeValue(baseValue, allModifiers);
            
            return withModifier - withoutModifier;
        }
        
        /// <summary>
        /// 验证修饰器是否有效
        /// </summary>
        /// <param name="modifier">修饰器</param>
        /// <returns>验证结果</returns>
        public ModifierValidationResult ValidateModifier(AttributeModifier modifier)
        {
            var result = new ModifierValidationResult { IsValid = true };
            
            if (modifier == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "修饰器不能为空";
                return result;
            }
            
            if (modifier.Id == Guid.Empty)
            {
                result.IsValid = false;
                result.ErrorMessage = "修饰器ID不能为空";
                return result;
            }
            
            if (modifier.OperationType == ModifierOperationType.Multiply && modifier.Value < 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "乘法修饰器的值不能为负数";
                return result;
            }
            
            if (modifier.OperationType == ModifierOperationType.Percentage && 
                (modifier.Value < -100 || modifier.Value > 1000))
            {
                result.IsValid = false;
                result.ErrorMessage = "百分比修饰器的值应在-100%到1000%之间";
                return result;
            }
            
            if (modifier.IsTemporary && modifier.Duration.IsInfinite)
            {
                result.IsValid = false;
                result.ErrorMessage = "临时修饰器不能有无限持续时间";
                return result;
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量验证修饰器
        /// </summary>
        /// <param name="modifiers">修饰器列表</param>
        /// <returns>验证结果列表</returns>
        public IEnumerable<ModifierValidationResult> ValidateModifiers(IEnumerable<AttributeModifier> modifiers)
        {
            return modifiers?.Select(ValidateModifier) ?? Enumerable.Empty<ModifierValidationResult>();
        }
        
        /// <summary>
        /// 检查修饰器冲突
        /// </summary>
        /// <param name="modifiers">修饰器列表</param>
        /// <returns>冲突检查结果</returns>
        public ConflictCheckResult CheckModifierConflicts(IEnumerable<AttributeModifier> modifiers)
        {
            var result = new ConflictCheckResult { HasConflicts = false };
            var modifierList = modifiers?.ToList() ?? new List<AttributeModifier>();
            
            // 检查重复的修饰器ID
            var duplicateIds = modifierList.GroupBy(m => m.Id)
                                          .Where(g => g.Count() > 1)
                                          .Select(g => g.Key)
                                          .ToList();
            
            if (duplicateIds.Any())
            {
                result.HasConflicts = true;
                result.ConflictMessages.Add($"发现重复的修饰器ID: {string.Join(", ", duplicateIds)}");
            }
            
            // 检查覆盖类型修饰器冲突
            var overrideModifiers = modifierList.Where(m => m.OperationType == ModifierOperationType.Override)
                                                .ToList();
            
            if (overrideModifiers.Count > 1)
            {
                result.HasConflicts = true;
                result.ConflictMessages.Add($"发现多个覆盖类型修饰器，只有最后一个会生效");
            }
            
            // 检查来源冲突（同一来源的同类型修饰器）
            var sourceConflicts = modifierList.GroupBy(m => new { m.Source.SourceId, m.OperationType })
                                             .Where(g => g.Count() > 1)
                                             .ToList();
            
            if (sourceConflicts.Any())
            {
                result.HasConflicts = true;
                foreach (var conflict in sourceConflicts)
                {
                    result.ConflictMessages.Add(
                        $"来源 {conflict.Key.SourceId} 有多个 {conflict.Key.OperationType} 类型的修饰器");
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 优化修饰器列表（移除冗余、合并同类型等）
        /// </summary>
        /// <param name="modifiers">修饰器列表</param>
        /// <returns>优化后的修饰器列表</returns>
        public IEnumerable<AttributeModifier> OptimizeModifiers(IEnumerable<AttributeModifier> modifiers)
        {
            var modifierList = modifiers?.Where(m => !m.IsExpired).ToList() ?? new List<AttributeModifier>();
            var optimized = new List<AttributeModifier>();
            
            // 按操作类型分组
            var groupedModifiers = modifierList.GroupBy(m => m.OperationType);
            
            foreach (var group in groupedModifiers)
            {
                switch (group.Key)
                {
                    case ModifierOperationType.Add:
                        // 合并同来源的加法修饰器
                        optimized.AddRange(MergeAdditiveModifiers(group));
                        break;
                        
                    case ModifierOperationType.Percentage:
                        // 合并同来源的百分比修饰器
                        optimized.AddRange(MergePercentageModifiers(group));
                        break;
                        
                    case ModifierOperationType.Override:
                        // 只保留最后一个覆盖修饰器
                        var lastOverride = group.OrderByDescending(m => m.ExecutionOrder).First();
                        optimized.Add(lastOverride);
                        break;
                        
                    default:
                        // 其他类型直接添加
                        optimized.AddRange(group);
                        break;
                }
            }
            
            return optimized;
        }
        
        /// <summary>
        /// 合并加法修饰器
        /// </summary>
        /// <param name="additiveModifiers">加法修饰器</param>
        /// <returns>合并后的修饰器</returns>
        private IEnumerable<AttributeModifier> MergeAdditiveModifiers(IEnumerable<AttributeModifier> additiveModifiers)
        {
            var grouped = additiveModifiers.GroupBy(m => m.Source.SourceId);
            
            foreach (var group in grouped)
            {
                if (group.Count() == 1)
                {
                    yield return group.First();
                }
                else
                {
                    // 合并同来源的加法修饰器
                    var first = group.First();
                    var totalValue = group.Sum(m => m.Value);
                    
                    yield return new AttributeModifier(
                        Guid.NewGuid(),
                        first.AttributeType,
                        ModifierOperationType.Add,
                        totalValue,
                        first.Source,
                        first.ExecutionOrder,
                        first.IsTemporary,
                        first.Duration
                    );
                }
            }
        }
        
        /// <summary>
        /// 合并百分比修饰器
        /// </summary>
        /// <param name="percentageModifiers">百分比修饰器</param>
        /// <returns>合并后的修饰器</returns>
        private IEnumerable<AttributeModifier> MergePercentageModifiers(IEnumerable<AttributeModifier> percentageModifiers)
        {
            var grouped = percentageModifiers.GroupBy(m => m.Source.SourceId);
            
            foreach (var group in grouped)
            {
                if (group.Count() == 1)
                {
                    yield return group.First();
                }
                else
                {
                    // 合并同来源的百分比修饰器
                    var first = group.First();
                    var totalPercentage = group.Sum(m => m.Value);
                    
                    yield return new AttributeModifier(
                        Guid.NewGuid(),
                        first.AttributeType,
                        ModifierOperationType.Percentage,
                        totalPercentage,
                        first.Source,
                        first.ExecutionOrder,
                        first.IsTemporary,
                        first.Duration
                    );
                }
            }
        }
    }
    
    /// <summary>
    /// 修饰器验证结果
    /// </summary>
    public class ModifierValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// 冲突检查结果
    /// </summary>
    public class ConflictCheckResult
    {
        /// <summary>
        /// 是否有冲突
        /// </summary>
        public bool HasConflicts { get; set; }
        
        /// <summary>
        /// 冲突消息列表
        /// </summary>
        public List<string> ConflictMessages { get; set; } = new List<string>();
    }
}