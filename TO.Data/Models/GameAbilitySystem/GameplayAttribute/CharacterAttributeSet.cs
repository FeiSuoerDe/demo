using System;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 角色属性集
    /// </summary>
    public class CharacterAttributeSet : AttributeSet
    {
        // 核心属性（六个基础属性）
        /// <summary>
        /// 智力属性
        /// </summary>
        public AttributeValue? Intelligence => GetAttribute(AttributeType.Intelligence);
        
        /// <summary>
        /// 感知属性
        /// </summary>
        public AttributeValue? Perception => GetAttribute(AttributeType.Perception);
        
        /// <summary>
        /// 魅力属性
        /// </summary>
        public AttributeValue? Charisma => GetAttribute(AttributeType.Charisma);
        
        /// <summary>
        /// 意志属性
        /// </summary>
        public AttributeValue? Will => GetAttribute(AttributeType.Will);
        
        /// <summary>
        /// 体质属性
        /// </summary>
        public AttributeValue? Constitution => GetAttribute(AttributeType.Constitution);
        
        /// <summary>
        /// 敏捷属性
        /// </summary>
        public AttributeValue? Agility => GetAttribute(AttributeType.Agility);
        
        // 派生属性（基于核心属性计算）
        /// <summary>
        /// 生命值属性
        /// </summary>
        public AttributeValue? LifeValue => GetAttribute(AttributeType.LifeValue);
        
        /// <summary>
        /// 精神值属性
        /// </summary>
        public AttributeValue? MentalValue => GetAttribute(AttributeType.MentalValue);
        
        /// <summary>
        /// 移动速度属性
        /// </summary>
        public AttributeValue? MovementSpeed => GetAttribute(AttributeType.MovementSpeed);
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">属性集ID</param>
        public CharacterAttributeSet(Guid id) : base(id)
        {
            InitializeCharacterAttributes();
        }
        
        /// <summary>
        /// 使用默认ID的构造函数
        /// </summary>
        public CharacterAttributeSet() : this(Guid.NewGuid())
        {
        }
        
        /// <summary>
        /// 初始化角色属性
        /// </summary>
        private void InitializeCharacterAttributes()
        {
            // 初始化核心属性（默认值）
            InitializeAttribute(AttributeType.Intelligence, 10);
            InitializeAttribute(AttributeType.Perception, 10);
            InitializeAttribute(AttributeType.Charisma, 10);
            InitializeAttribute(AttributeType.Will, 10);
            InitializeAttribute(AttributeType.Constitution, 10);
            InitializeAttribute(AttributeType.Agility, 10);
            
            // 初始化派生属性（将通过计算引擎自动计算）
            InitializeAttribute(AttributeType.LifeValue, 0);
            InitializeAttribute(AttributeType.MentalValue, 0);
            InitializeAttribute(AttributeType.MovementSpeed, 0);
            
            // 计算初始派生属性值
            RecalculateDerivedAttributes();
        }
        
        /// <summary>
        /// 重新计算派生属性
        /// </summary>
        public void RecalculateDerivedAttributes()
        {
            // 生命值 = 体质 * 10
            UpdateAttribute(AttributeType.LifeValue, Constitution.CurrentValue * 10);
            
            // 精神值 = 意志 * 8
            UpdateAttribute(AttributeType.MentalValue, Will.CurrentValue * 8);
            
            // 移动速度 = 敏捷 * 0.5
            UpdateAttribute(AttributeType.MovementSpeed, Agility.CurrentValue * 0.5f);
        }
        
        /// <summary>
        /// 属性变化时的回调
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        protected override void OnAttributeChanged(AttributeType attributeType, float oldValue, float newValue)
        {
            base.OnAttributeChanged(attributeType, oldValue, newValue);
            
            // 当核心属性变化时，重新计算派生属性
            if (IsCoreAttribute(attributeType))
            {
                RecalculateDerivedAttributes();
            }
        }
        
        /// <summary>
        /// 检查是否为核心属性
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>是否为核心属性</returns>
        private bool IsCoreAttribute(AttributeType attributeType)
        {
            return attributeType == AttributeType.Intelligence ||
                   attributeType == AttributeType.Perception ||
                   attributeType == AttributeType.Charisma ||
                   attributeType == AttributeType.Will ||
                   attributeType == AttributeType.Constitution ||
                   attributeType == AttributeType.Agility;
        }
        
        /// <summary>
        /// 设置核心属性值
        /// </summary>
        /// <param name="intelligence">智力</param>
        /// <param name="perception">感知</param>
        /// <param name="charisma">魅力</param>
        /// <param name="will">意志</param>
        /// <param name="constitution">体质</param>
        /// <param name="agility">敏捷</param>
        public void SetCoreAttributes(float intelligence, float perception, float charisma, 
                                    float will, float constitution, float agility)
        {
            SetAttribute(AttributeType.Intelligence, intelligence);
            SetAttribute(AttributeType.Perception, perception);
            SetAttribute(AttributeType.Charisma, charisma);
            SetAttribute(AttributeType.Will, will);
            SetAttribute(AttributeType.Constitution, constitution);
            SetAttribute(AttributeType.Agility, agility);
        }
        
        /// <summary>
        /// 获取属性总和（用于角色评估）
        /// </summary>
        /// <returns>核心属性总和</returns>
        public float GetCoreAttributeSum()
        {
            return Intelligence.CurrentValue +
                   Perception.CurrentValue +
                   Charisma.CurrentValue +
                   Will.CurrentValue +
                   Constitution.CurrentValue +
                   Agility.CurrentValue;
        }
        
        /// <summary>
        /// 获取属性平均值
        /// </summary>
        /// <returns>核心属性平均值</returns>
        public float GetCoreAttributeAverage()
        {
            return GetCoreAttributeSum() / 6f;
        }
        
        /// <summary>
        /// 检查角色是否健康（生命值大于0）
        /// </summary>
        /// <returns>是否健康</returns>
        public bool IsAlive()
        {
            return LifeValue.CurrentValue > 0;
        }
        
        /// <summary>
        /// 检查角色是否有足够的精神值
        /// </summary>
        /// <param name="requiredMental">所需精神值</param>
        /// <returns>是否有足够精神值</returns>
        public bool HasEnoughMental(float requiredMental)
        {
            return MentalValue.CurrentValue >= requiredMental;
        }
        
        /// <summary>
        /// 消耗精神值
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        public bool ConsumeMental(float amount)
        {
            if (!HasEnoughMental(amount))
                return false;
                
            var newValue = Math.Max(0, MentalValue.CurrentValue - amount);
            UpdateAttribute(AttributeType.MentalValue, newValue);
            return true;
        }
        
        /// <summary>
        /// 恢复精神值
        /// </summary>
        /// <param name="amount">恢复量</param>
        public void RestoreMental(float amount)
        {
            var maxMental = Will.CurrentValue * 8; // 最大精神值基于意志计算
            var newValue = Math.Min(maxMental, MentalValue.CurrentValue + amount);
            UpdateAttribute(AttributeType.MentalValue, newValue);
        }
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public float TakeDamage(float damage)
        {
            var actualDamage = Math.Min(damage, LifeValue.CurrentValue);
            var newValue = Math.Max(0, LifeValue.CurrentValue - actualDamage);
            UpdateAttribute(AttributeType.LifeValue, newValue);
            return actualDamage;
        }
        
        /// <summary>
        /// 治疗生命值
        /// </summary>
        /// <param name="healAmount">治疗量</param>
        /// <returns>实际治疗量</returns>
        public float Heal(float healAmount)
        {
            var maxLife = Constitution.CurrentValue * 10; // 最大生命值基于体质计算
            var actualHeal = Math.Min(healAmount, maxLife - LifeValue.CurrentValue);
            var newValue = Math.Min(maxLife, LifeValue.CurrentValue + actualHeal);
            UpdateAttribute(AttributeType.LifeValue, newValue);
            return actualHeal;
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>格式化的角色属性信息</returns>
        public override string ToString()
        {
            return $"Character Attributes - INT:{Intelligence.CurrentValue} PER:{Perception.CurrentValue} " +
                   $"CHA:{Charisma.CurrentValue} WIL:{Will.CurrentValue} CON:{Constitution.CurrentValue} " +
                   $"AGI:{Agility.CurrentValue} | Life:{LifeValue.CurrentValue} Mental:{MentalValue.CurrentValue} " +
                   $"Speed:{MovementSpeed.CurrentValue}";
        }
    }
}