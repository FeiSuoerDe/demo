using Godot;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using System.Text;


namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 属性集
    /// </summary>
    public class AttributeSet
    {
        /// <summary>
        /// 属性集唯一标识
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// 属性列表 - 优化后移除冗余的AttributeType键
        /// </summary>
        public List<AttributeValue> Attributes { get; }
        
        /// <summary>
        /// 应用的效果列表
        /// </summary>
        public List<AttributeEffect?> AppliedEffects { get; }
        
        /// <summary>
        /// 属性变化事件
        /// </summary>
        public event Action<AttributeType, float, float> AttributeChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attributes"></param>
        public AttributeSet(List<AttributeValue> attributes)
        {
            Id = Guid.NewGuid();
            Attributes = attributes;
            AppliedEffects = [];
            AttributeChanged = delegate { };
        }


        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <returns>属性值，如果不存在则返回null</returns>
        public AttributeValue? GetAttribute(AttributeType type)
        {
            return Attributes.FirstOrDefault(attr => attr.AttributeType == type);
        }
        
        /// <summary>
        /// 检查是否拥有指定属性
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <returns>是否拥有该属性</returns>
        public bool HasAttribute(AttributeType type)
        {
            return Attributes.Any(attr => attr.AttributeType == type);
        }
        
        /// <summary>
        /// 获取所有属性类型
        /// </summary>
        /// <returns>属性类型集合</returns>
        public IEnumerable<AttributeType> GetAllAttributeTypes()
        {
            return Attributes.Select(attr => attr.AttributeType);
        }
        
        /// <summary>
        /// 获取所有属性值
        /// </summary>
        /// <returns>属性值集合</returns>
        public IEnumerable<AttributeValue?> GetAllAttributeValues()
        {
            return Attributes;
        }
        
        /// <summary>
        /// 设置属性基础值
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="value">基础值</param>
        public void SetAttribute(AttributeType type, float value)
        {
            var oldValue = GetAttributeCurrentValue(type);
            
            var attribute = GetAttribute(type);
            if (attribute != null)
            {
                attribute.SetBaseValue(value);
            }
            else
            {
                Attributes.Add(new AttributeValue(type, value));
            }
            
            // 重新计算当前值
            RecalculateAttribute(type);
            
            var newValue = GetAttributeCurrentValue(type);
            OnAttributeChanged(type, oldValue, newValue);
        }
        
        /// <summary>
        /// 设置属性值范围
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        public void SetAttributeRange(AttributeType type, float minValue, float maxValue)
        {
            var attribute = GetAttribute(type);
            if (attribute != null)
            {
                attribute.SetValueRange(minValue, maxValue);
                
                // 重新计算当前值以应用新的范围限制
                RecalculateAttribute(type);
            }
        }
        
        /// <summary>
        /// 获取属性当前值
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <returns>当前值</returns>
        public float GetAttributeCurrentValue(AttributeType type)
        {
            var attribute = GetAttribute(type);
            return attribute?.CurrentValue ?? 0f;
        }
        
        /// <summary>
        /// 获取属性基础值
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <returns>基础值</returns>
        public float GetAttributeBaseValue(AttributeType type)
        {
            var attribute = GetAttribute(type);
            return attribute?.BaseValue ?? 0f;
        }
        
        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="baseValue">基础值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        protected void InitializeAttribute(AttributeType type, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            var attribute = GetAttribute(type);
            if (attribute == null)
            {
                Attributes.Add(new AttributeValue(type, baseValue, minValue, maxValue));
            }
            else
            {
                attribute.SetValueRange(minValue, maxValue);
                attribute.SetBaseValue(baseValue);
            }
        }
        
        /// <summary>
        /// 更新属性值（内部使用）
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="value">新值</param>
        protected void UpdateAttribute(AttributeType type, float value)
        {
            var attribute = GetAttribute(type);
            if (attribute != null)
            {
                var oldValue = attribute.CurrentValue;
                attribute.SetCurrentValue(value);
                OnAttributeChanged(type, oldValue, value);
            }
        }
        
        /// <summary>
        /// 应用效果
        /// </summary>
        /// <param name="effect">要应用的效果</param>
        /// <returns>是否成功应用</returns>
        public virtual bool ApplyEffect(AttributeEffect? effect)
        {
            if (effect == null)
                return false;
                
            // 检查是否已存在相同效果
            var existingEffect = AppliedEffects.FirstOrDefault(e => e.Id == effect.Id);
            if (existingEffect != null)
            {
                return HandleExistingEffect(existingEffect, effect);
            }
            
            // 添加新效果
            AppliedEffects.Add(effect);
            
            // 重新计算受影响的属性
            var affectedAttributes = effect.Modifiers.Select(m => m.AttributeType).Distinct();
            foreach (var attributeType in affectedAttributes)
            {
                RecalculateAttribute(attributeType);
            }
            
            return true;
        }
        
        /// <summary>
        /// 移除效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>是否成功移除</returns>
        public virtual bool RemoveEffect(Guid effectId)
        {
            var effect = AppliedEffects.FirstOrDefault(e => e.Id == effectId);
            if (effect == null)
                return false;
                
            AppliedEffects.Remove(effect);
            
            // 重新计算受影响的属性
            var affectedAttributes = effect.Modifiers.Select(m => m.AttributeType).Distinct();
            foreach (AttributeType attributeType in affectedAttributes)
            {
                RecalculateAttribute(attributeType);
            }
            
            return true;
        }
        
        /// <summary>
        /// 更新所有效果的持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateEffects(float deltaTime)
        {
            var expiredEffects = new List<AttributeEffect?>();
            
            foreach (var effect in AppliedEffects)
            {
                effect?.UpdateDuration(deltaTime);
                if (effect is { IsExpired: true })
                {
                    expiredEffects.Add(effect);
                }
            }
            
            // 移除过期效果
            foreach (var expiredEffect in expiredEffects.OfType<AttributeEffect>())
            {
                RemoveEffect(expiredEffect.Id);
            }
        }

        /// <summary>
        /// 重新计算指定属性的当前值
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        protected virtual void RecalculateAttribute(AttributeType attributeType)
        {
            var attribute = GetAttribute(attributeType);
            if (attribute == null)
                return;
                
            var baseValue = attribute.BaseValue;
            var currentValue = baseValue;
            
            // 获取所有影响该属性的修饰器
            var modifiers = AppliedEffects
                .SelectMany(e => e.Modifiers)
                .Where(m => m.AttributeType == attributeType && !m.IsExpired)
                .OrderBy(m => m.ExecutionOrder)
                .ToList();
            
            // 按操作类型分组应用修饰器
            var addModifiers = modifiers.Where(m => m.OperationType == ModifierOperationType.Add).ToList();
            var multiplyModifiers = modifiers.Where(m => m.OperationType == ModifierOperationType.Multiply).ToList();
            var percentageModifiers = modifiers.Where(m => m.OperationType == ModifierOperationType.Percentage).ToList();
            var overrideModifiers = modifiers.Where(m => m.OperationType == ModifierOperationType.Override).ToList();
            
            // 如果有覆盖修饰器，使用最后一个
            if (overrideModifiers.Count != 0)
            {
                currentValue = overrideModifiers.Last().Value;
            }
            else
            {
                // 先应用加法修饰器
                foreach (var modifier in addModifiers)
                {
                    currentValue += modifier.Value;
                }
                
                // 再应用乘法修饰器
                foreach (var modifier in multiplyModifiers)
                {
                    currentValue *= modifier.Value;
                }
                
                // 最后应用百分比修饰器
                foreach (var modifier in percentageModifiers)
                {
                    currentValue *= (1 + modifier.Value / 100);
                }
            }
            
            var oldValue = attribute.CurrentValue;
            attribute.SetCurrentValue(currentValue);
            
            if (Math.Abs(oldValue - currentValue) > float.Epsilon)
            {
                OnAttributeChanged(attributeType, oldValue, currentValue);
            }
        }
        
        /// <summary>
        /// 处理已存在的效果
        /// </summary>
        /// <param name="existingEffect">已存在的效果</param>
        /// <param name="newEffect">新效果</param>
        /// <returns>是否成功处理</returns>
        protected virtual bool HandleExistingEffect(AttributeEffect existingEffect, AttributeEffect? newEffect)
        {
            switch (existingEffect.StackingType)
            {
                case EffectStackingType.NoStack:
                    return false; // 不允许堆叠
                    
                case EffectStackingType.Stack:
                    return existingEffect.AddStack(); // 增加层数
                    
                case EffectStackingType.Replace:
                    RemoveEffect(existingEffect.Id);
                    AppliedEffects.Add(newEffect);
                    return true;
                    
                case EffectStackingType.Duration:
                    existingEffect.RefreshDuration();
                    return true;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 属性变化时的回调
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        private void OnAttributeChanged(AttributeType attributeType, float oldValue, float newValue)
        {
            AttributeChanged?.Invoke(attributeType, oldValue, newValue);
        }
        
        /// <summary>
        /// 获取所有属性的副本
        /// </summary>
        /// <returns>属性列表的副本</returns>
        public List<AttributeValue?> GetAllAttributes()
        {
            return new List<AttributeValue?>(Attributes);
        }
        
        /// <summary>
        /// 获取所有属性的字典形式（为了兼容性）
        /// </summary>
        /// <returns>属性字典</returns>
        public Dictionary<AttributeType, AttributeValue?> GetAllAttributesAsDictionary()
        {
            return Attributes.ToDictionary(attr => attr.AttributeType, attr => attr);
        }
        
        /// <summary>
        /// 获取所有应用的效果
        /// </summary>
        /// <returns>效果列表的副本</returns>
        public List<AttributeEffect?> GetAppliedEffects()
        {
            return new List<AttributeEffect?>(AppliedEffects);
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"AttributeSet Id: {Id}");
            sb.AppendLine("Attributes:");
            foreach (var attr in Attributes)
            {
                sb.AppendLine($"  {attr.AttributeType}: Base = {attr.BaseValue}, Current = {attr.CurrentValue}");
            }
            sb.AppendLine("Applied Effects:");
            foreach (var effect in AppliedEffects)
            {
                sb.AppendLine($"  Effect Id: {effect?.Id ?? Guid.Empty}");
            }
            return sb.ToString();
        }
    }
}