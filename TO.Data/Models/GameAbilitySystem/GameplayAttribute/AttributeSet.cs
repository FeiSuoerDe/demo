using Godot;
using TO.Commons.Enums.Game;
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
        /// 属性变化事件
        /// </summary>
        public event Action<AttributeType, float, float> AttributeChanged;
        
        public event Action<AttributeType, float, float> AttributeRangeChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attributes"></param>
        public AttributeSet(List<AttributeValue> attributes)
        {
            Id = Guid.NewGuid();
            Attributes = attributes;
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
        
        public void SetAttributeCurrentValue(AttributeType type, float value)
        {
            var oldValue = GetAttributeCurrentValue(type);
            
            var attribute = GetAttribute(type);
            if (attribute != null)
            {
                attribute.SetCurrentValue(value);
            }
            else
            {
                Attributes.Add(new AttributeValue(type, value));
            }
            
            var newValue = GetAttributeCurrentValue(type);
            OnAttributeChanged(type, oldValue, newValue);
        }
        
        /// <summary>
        /// 设置属性基础值
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="value">基础值</param>
        public void SetAttributeBaseValue(AttributeType type, float value)
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
            
            var newValue = GetAttributeBaseValue(type);
            OnAttributeChanged(type, oldValue, newValue);
        }
        
        
        
        /// <summary>
        /// 设置属性值范围
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        public void SetAttributeMinValue(AttributeType type, float minValue)
        {
            var attribute = GetAttribute(type);
            if (attribute == null) return;
            attribute.SetMinValue(minValue);
            OnAttributeRangeChanged(type, minValue, attribute.MaxValue);
        }
        
        public void SetAttributeMaxValue(AttributeType type,float maxValue)
        {
            var attribute = GetAttribute(type);
            if (attribute == null) return;
            attribute.SetMaxValue(maxValue);
            OnAttributeRangeChanged(type, attribute.MinValue, maxValue);
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
                attribute.SetMaxValue(maxValue);
                attribute.SetMinValue(minValue);
                attribute.SetBaseValue(baseValue);
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
        
        private void OnAttributeRangeChanged(AttributeType attributeType, float minValue, float maxValue)
        {
            AttributeRangeChanged?.Invoke(attributeType, minValue, maxValue);
        }
        
        /// <summary>
        /// 获取所有属性的副本
        /// </summary>
        /// <returns>属性列表的副本</returns>
        public List<AttributeValue?> GetAllAttributes()
        {
            return [..Attributes];
        }
        
        /// <summary>
        /// 获取所有属性的字典形式（为了兼容性）
        /// </summary>
        /// <returns>属性字典</returns>
        public Dictionary<AttributeType, AttributeValue> GetAllAttributesAsDictionary()
        {
            return Attributes.ToDictionary(attr => attr.AttributeType, attr => attr);
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
            return sb.ToString();
        }
    }
}