using System;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 属性值类，包含基础值和当前值
    /// </summary>
    public class AttributeValue
    {
        /// <summary>
        /// 基础值
        /// </summary>
        public float BaseValue { get; private set; }
        
        /// <summary>
        /// 当前值（经过修饰器计算后的值）
        /// </summary>
        public float CurrentValue { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseValue">基础值</param>
        public AttributeValue(float baseValue)
        {
            BaseValue = baseValue;
            CurrentValue = baseValue;
        }
        
        /// <summary>
        /// 设置基础值
        /// </summary>
        /// <param name="value">新的基础值</param>
        public void SetBaseValue(float value)
        {
            BaseValue = value;
            RecalculateCurrentValue();
        }
        
        /// <summary>
        /// 设置当前值
        /// </summary>
        /// <param name="value">新的当前值</param>
        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
        }
        
        /// <summary>
        /// 重新计算当前值
        /// </summary>
        private void RecalculateCurrentValue()
        {
            // 重新计算当前值的逻辑
            // 在实际使用中，这里会应用所有修饰器的效果
            CurrentValue = BaseValue;
        }
        
        /// <summary>
        /// 获取属性值的字符串表示
        /// </summary>
        /// <returns>格式化的属性值字符串</returns>
        public override string ToString()
        {
            return $"Base: {BaseValue}, Current: {CurrentValue}";
        }
        
        /// <summary>
        /// 判断两个属性值是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is AttributeValue other)
            {
                return Math.Abs(BaseValue - other.BaseValue) < float.Epsilon &&
                       Math.Abs(CurrentValue - other.CurrentValue) < float.Epsilon;
            }
            return false;
        }
        
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(BaseValue, CurrentValue);
        }
    }
}