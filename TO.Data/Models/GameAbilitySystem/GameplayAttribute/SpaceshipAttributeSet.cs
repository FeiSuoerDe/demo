using System;
using TO.Commons.Enums.Game;

namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    /// <summary>
    /// 飞船属性集
    /// </summary>
    public class SpaceshipAttributeSet : AttributeSet
    {
        // 核心属性
        /// <summary>
        /// 推进力属性
        /// </summary>
        public AttributeValue? Thrust => GetAttribute(AttributeType.Thrust);
        
        /// <summary>
        /// 护盾属性
        /// </summary>
        public AttributeValue? Shield => GetAttribute(AttributeType.Shield);
        
        /// <summary>
        /// 装甲属性
        /// </summary>
        public AttributeValue? Armor => GetAttribute(AttributeType.Armor);
        
        /// <summary>
        /// 能量属性
        /// </summary>
        public AttributeValue? Energy => GetAttribute(AttributeType.Energy);
        
        /// <summary>
        /// 机动性属性
        /// </summary>
        public AttributeValue? Maneuverability => GetAttribute(AttributeType.Maneuverability);
        
        /// <summary>
        /// 传感器属性
        /// </summary>
        public AttributeValue? Sensors => GetAttribute(AttributeType.Sensors);
        
        // 规格属性
        /// <summary>
        /// 质量属性
        /// </summary>
        public AttributeValue? Mass => GetAttribute(AttributeType.Mass);
        
        /// <summary>
        /// 长度属性
        /// </summary>
        public AttributeValue? Length => GetAttribute(AttributeType.Length);
        
        /// <summary>
        /// 宽度属性
        /// </summary>
        public AttributeValue? Width => GetAttribute(AttributeType.Width);
        
        /// <summary>
        /// 高度属性
        /// </summary>
        public AttributeValue? Height => GetAttribute(AttributeType.Height);
        
        /// <summary>
        /// 货舱容量属性
        /// </summary>
        public AttributeValue? CargoCapacity => GetAttribute(AttributeType.CargoCapacity);
        
        /// <summary>
        /// 燃料容量属性
        /// </summary>
        public AttributeValue? FuelCapacity => GetAttribute(AttributeType.FuelCapacity);
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">属性集ID</param>
        public SpaceshipAttributeSet(Guid id) : base(id)
        {
            InitializeSpaceshipAttributes();
        }
        
        /// <summary>
        /// 使用默认ID的构造函数
        /// </summary>
        public SpaceshipAttributeSet() : this(Guid.NewGuid())
        {
        }
        
        /// <summary>
        /// 初始化飞船属性
        /// </summary>
        private void InitializeSpaceshipAttributes()
        {
            // 初始化核心属性（默认值）
            InitializeAttribute(AttributeType.Thrust, 100);
            InitializeAttribute(AttributeType.Shield, 50);
            InitializeAttribute(AttributeType.Armor, 30);
            InitializeAttribute(AttributeType.Energy, 200);
            InitializeAttribute(AttributeType.Maneuverability, 10);
            InitializeAttribute(AttributeType.Sensors, 15);
            
            // 初始化规格属性（默认值）
            InitializeAttribute(AttributeType.Mass, 1000);
            InitializeAttribute(AttributeType.Length, 50);
            InitializeAttribute(AttributeType.Width, 20);
            InitializeAttribute(AttributeType.Height, 15);
            InitializeAttribute(AttributeType.CargoCapacity, 100);
            InitializeAttribute(AttributeType.FuelCapacity, 500);
        }
        
        /// <summary>
        /// 设置飞船核心属性
        /// </summary>
        /// <param name="thrust">推进力</param>
        /// <param name="shield">护盾</param>
        /// <param name="armor">装甲</param>
        /// <param name="energy">能量</param>
        /// <param name="maneuverability">机动性</param>
        /// <param name="sensors">传感器</param>
        public void SetCoreAttributes(float thrust, float shield, float armor, 
                                    float energy, float maneuverability, float sensors)
        {
            SetAttribute(AttributeType.Thrust, thrust);
            SetAttribute(AttributeType.Shield, shield);
            SetAttribute(AttributeType.Armor, armor);
            SetAttribute(AttributeType.Energy, energy);
            SetAttribute(AttributeType.Maneuverability, maneuverability);
            SetAttribute(AttributeType.Sensors, sensors);
        }
        
        /// <summary>
        /// 设置飞船规格属性
        /// </summary>
        /// <param name="mass">质量</param>
        /// <param name="length">长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="cargoCapacity">货舱容量</param>
        /// <param name="fuelCapacity">燃料容量</param>
        public void SetSpecificationAttributes(float mass, float length, float width, 
                                             float height, float cargoCapacity, float fuelCapacity)
        {
            SetAttribute(AttributeType.Mass, mass);
            SetAttribute(AttributeType.Length, length);
            SetAttribute(AttributeType.Width, width);
            SetAttribute(AttributeType.Height, height);
            SetAttribute(AttributeType.CargoCapacity, cargoCapacity);
            SetAttribute(AttributeType.FuelCapacity, fuelCapacity);
        }
        
        /// <summary>
        /// 计算飞船体积
        /// </summary>
        /// <returns>飞船体积</returns>
        public float CalculateVolume()
        {
            return Length.CurrentValue * Width.CurrentValue * Height.CurrentValue;
        }
        
        /// <summary>
        /// 计算推重比
        /// </summary>
        /// <returns>推重比</returns>
        public float CalculateThrustToWeightRatio()
        {
            if (Mass.CurrentValue <= 0)
                return 0;
            return Thrust.CurrentValue / Mass.CurrentValue;
        }
        
        /// <summary>
        /// 计算机动性评分
        /// </summary>
        /// <returns>机动性评分</returns>
        public float CalculateManeuverabilityScore()
        {
            var thrustToWeight = CalculateThrustToWeightRatio();
            return Maneuverability.CurrentValue * thrustToWeight;
        }
        
        /// <summary>
        /// 计算防御评分
        /// </summary>
        /// <returns>防御评分</returns>
        public float CalculateDefenseScore()
        {
            return Shield.CurrentValue + Armor.CurrentValue;
        }
        
        /// <summary>
        /// 计算综合战斗力评分
        /// </summary>
        /// <returns>战斗力评分</returns>
        public float CalculateCombatRating()
        {
            var offense = Thrust.CurrentValue + Energy.CurrentValue;
            var defense = CalculateDefenseScore();
            var mobility = CalculateManeuverabilityScore();
            var detection = Sensors.CurrentValue;
            
            return (offense * 0.3f) + (defense * 0.3f) + (mobility * 0.25f) + (detection * 0.15f);
        }
        
        /// <summary>
        /// 检查护盾是否激活
        /// </summary>
        /// <returns>护盾是否激活</returns>
        public bool IsShieldActive()
        {
            return Shield.CurrentValue > 0;
        }
        
        /// <summary>
        /// 检查是否有足够的能量
        /// </summary>
        /// <param name="requiredEnergy">所需能量</param>
        /// <returns>是否有足够能量</returns>
        public bool HasEnoughEnergy(float requiredEnergy)
        {
            return Energy.CurrentValue >= requiredEnergy;
        }
        
        /// <summary>
        /// 消耗能量
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        public bool ConsumeEnergy(float amount)
        {
            if (!HasEnoughEnergy(amount))
                return false;
                
            var newValue = Math.Max(0, Energy.CurrentValue - amount);
            UpdateAttribute(AttributeType.Energy, newValue);
            return true;
        }
        
        /// <summary>
        /// 恢复能量
        /// </summary>
        /// <param name="amount">恢复量</param>
        public void RestoreEnergy(float amount)
        {
            var maxEnergy = GetAttribute(AttributeType.Energy).BaseValue; // 使用基础值作为最大值
            var newValue = Math.Min(maxEnergy, Energy.CurrentValue + amount);
            UpdateAttribute(AttributeType.Energy, newValue);
        }
        
        /// <summary>
        /// 护盾受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>穿透到装甲的伤害</returns>
        public float TakeShieldDamage(float damage)
        {
            if (Shield.CurrentValue <= 0)
                return damage; // 护盾已破，伤害直接穿透
                
            var shieldDamage = Math.Min(damage, Shield.CurrentValue);
            var newShieldValue = Math.Max(0, Shield.CurrentValue - shieldDamage);
            UpdateAttribute(AttributeType.Shield, newShieldValue);
            
            return damage - shieldDamage; // 返回穿透伤害
        }
        
        /// <summary>
        /// 装甲受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public float TakeArmorDamage(float damage)
        {
            var actualDamage = Math.Min(damage, Armor.CurrentValue);
            var newValue = Math.Max(0, Armor.CurrentValue - actualDamage);
            UpdateAttribute(AttributeType.Armor, newValue);
            return actualDamage;
        }
        
        /// <summary>
        /// 受到总伤害（先护盾后装甲）
        /// </summary>
        /// <param name="damage">总伤害值</param>
        /// <returns>实际造成的结构伤害</returns>
        public float TakeDamage(float damage)
        {
            var remainingDamage = TakeShieldDamage(damage);
            if (remainingDamage > 0)
            {
                return TakeArmorDamage(remainingDamage);
            }
            return 0;
        }
        
        /// <summary>
        /// 修复护盾
        /// </summary>
        /// <param name="repairAmount">修复量</param>
        /// <returns>实际修复量</returns>
        public float RepairShield(float repairAmount)
        {
            var maxShield = GetAttribute(AttributeType.Shield).BaseValue;
            var actualRepair = Math.Min(repairAmount, maxShield - Shield.CurrentValue);
            var newValue = Math.Min(maxShield, Shield.CurrentValue + actualRepair);
            UpdateAttribute(AttributeType.Shield, newValue);
            return actualRepair;
        }
        
        /// <summary>
        /// 修复装甲
        /// </summary>
        /// <param name="repairAmount">修复量</param>
        /// <returns>实际修复量</returns>
        public float RepairArmor(float repairAmount)
        {
            var maxArmor = GetAttribute(AttributeType.Armor).BaseValue;
            var actualRepair = Math.Min(repairAmount, maxArmor - Armor.CurrentValue);
            var newValue = Math.Min(maxArmor, Armor.CurrentValue + actualRepair);
            UpdateAttribute(AttributeType.Armor, newValue);
            return actualRepair;
        }
        
        /// <summary>
        /// 检查飞船是否被摧毁
        /// </summary>
        /// <returns>是否被摧毁</returns>
        public bool IsDestroyed()
        {
            return Shield.CurrentValue <= 0 && Armor.CurrentValue <= 0;
        }
        
        /// <summary>
        /// 检查飞船是否处于危险状态
        /// </summary>
        /// <returns>是否处于危险状态</returns>
        public bool IsInDanger()
        {
            var maxShield = GetAttribute(AttributeType.Shield).BaseValue;
            var maxArmor = GetAttribute(AttributeType.Armor).BaseValue;
            
            var shieldPercent = maxShield > 0 ? Shield.CurrentValue / maxShield : 0;
            var armorPercent = maxArmor > 0 ? Armor.CurrentValue / maxArmor : 0;
            
            return shieldPercent < 0.25f || armorPercent < 0.25f;
        }
        
        /// <summary>
        /// 检查货舱是否有足够空间
        /// </summary>
        /// <param name="requiredSpace">所需空间</param>
        /// <param name="currentCargo">当前货物量</param>
        /// <returns>是否有足够空间</returns>
        public bool HasEnoughCargoSpace(float requiredSpace, float currentCargo = 0)
        {
            return (currentCargo + requiredSpace) <= CargoCapacity.CurrentValue;
        }
        
        /// <summary>
        /// 检查燃料是否充足
        /// </summary>
        /// <param name="requiredFuel">所需燃料</param>
        /// <param name="currentFuel">当前燃料</param>
        /// <returns>是否燃料充足</returns>
        public bool HasEnoughFuel(float requiredFuel, float currentFuel)
        {
            return currentFuel >= requiredFuel;
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>格式化的飞船属性信息</returns>
        public override string ToString()
        {
            return $"Spaceship Attributes - Thrust:{Thrust.CurrentValue} Shield:{Shield.CurrentValue} " +
                   $"Armor:{Armor.CurrentValue} Energy:{Energy.CurrentValue} Maneuver:{Maneuverability.CurrentValue} " +
                   $"Sensors:{Sensors.CurrentValue} | Mass:{Mass.CurrentValue} Size:{Length.CurrentValue}x{Width.CurrentValue}x{Height.CurrentValue} " +
                   $"Cargo:{CargoCapacity.CurrentValue} Fuel:{FuelCapacity.CurrentValue}";
        }
    }
}