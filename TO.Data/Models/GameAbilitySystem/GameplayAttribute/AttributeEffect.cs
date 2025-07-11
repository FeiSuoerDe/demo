using TO.Commons.Enums.Game;


namespace TO.Data.Models.GameAbilitySystem.GameplayAttribute
{
    // EffectType, EffectTags, EffectStackingType 枚举已移动到 TO.Commons.Enums.GameAbilitySystemEnums
    
    /// <summary>
    /// 属性效果类
    /// </summary>
    public class AttributeEffect
    {
        /// <summary>
        /// 效果唯一标识
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// 效果名称
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// 效果描述
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// 修饰器列表
        /// </summary>
        public List<AttributeModifier> Modifiers { get; private set; }
        
        /// <summary>
        /// 持续时间
        /// </summary>
        public Duration Duration { get; }
        
        /// <summary>
        /// 效果类型
        /// </summary>
        public EffectType EffectType { get; }
        
        /// <summary>
        /// 效果标签
        /// </summary>
        public EffectTags Tags { get; }
        
        /// <summary>
        /// 堆叠类型
        /// </summary>
        public EffectStackingType StackingType { get; }
        
        /// <summary>
        /// 最大堆叠层数
        /// </summary>
        public int MaxStacks { get; }
        
        /// <summary>
        /// 当前堆叠层数
        /// </summary>
        public int CurrentStacks { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">效果ID</param>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="modifiers">修饰器列表</param>
        /// <param name="duration">持续时间</param>
        /// <param name="effectType">效果类型</param>
        /// <param name="tags">效果标签</param>
        /// <param name="stackingType">堆叠类型</param>
        /// <param name="maxStacks">最大堆叠层数</param>
        public AttributeEffect(Guid id, string name, string description, List<AttributeModifier> modifiers, 
                    Duration duration, EffectType effectType, EffectTags tags, 
                    EffectStackingType stackingType = EffectStackingType.Replace, int maxStacks = 1)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Modifiers = modifiers ?? new List<AttributeModifier>();
            Duration = duration ?? Duration.Infinite;
            EffectType = effectType;
            Tags = tags;
            StackingType = stackingType;
            MaxStacks = maxStacks;
            CurrentStacks = 1; // 初始为1层
        }
        
        /// <summary>
        /// 检查效果是否已过期
        /// </summary>
        public bool IsExpired => Duration.IsExpired;
        
        /// <summary>
        /// 更新持续时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateDuration(float deltaTime)
        {
            if (!Duration.IsInfinite)
            {
                Duration.Update(deltaTime);
            }
            
            // 同时更新所有修饰器的持续时间
            foreach (var modifier in Modifiers.Where(m => m.IsTemporary))
            {
                modifier.UpdateDuration(deltaTime);
            }
        }
        
        /// <summary>
        /// 添加层数
        /// </summary>
        /// <returns>是否成功添加</returns>
        public bool AddStack()
        {
            if (StackingType == EffectStackingType.NoStack || CurrentStacks >= MaxStacks)
                return false;
                
            CurrentStacks++;
            return true;
        }
        
        /// <summary>
        /// 移除层数
        /// </summary>
        /// <returns>是否成功移除</returns>
        public bool RemoveStack()
        {
            if (CurrentStacks <= 1)
                return false;
                
            CurrentStacks--;
            return true;
        }
        
        /// <summary>
        /// 重置持续时间
        /// </summary>
        public void RefreshDuration()
        {
            if (!Duration.IsInfinite)
            {
                Duration.Reset();
            }
        }
        
        /// <summary>
        /// 添加修饰器
        /// </summary>
        /// <param name="modifier">要添加的修饰器</param>
        public void AddModifier(AttributeModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));
                
            Modifiers.Add(modifier);
        }
        
        /// <summary>
        /// 移除修饰器
        /// </summary>
        /// <param name="modifierId">修饰器ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveModifier(Guid modifierId)
        {
            var modifier = Modifiers.FirstOrDefault(m => m.Id == modifierId);
            if (modifier != null)
            {
                return Modifiers.Remove(modifier);
            }
            return false;
        }
        
        /// <summary>
        /// 获取指定属性类型的所有修饰器
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>修饰器列表</returns>
        public IEnumerable<AttributeModifier> GetModifiersForAttribute(AttributeType attributeType)
        {
            return Modifiers.Where(m => m.AttributeType == attributeType);
        }
        
        /// <summary>
        /// 判断两个效果是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is AttributeEffect other)
            {
                return Id == other.Id;
            }
            return false;
        }
        
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>格式化的效果信息字符串</returns>
        public override string ToString()
        {
            var stackInfo = MaxStacks > 1 ? $" ({CurrentStacks}/{MaxStacks})" : "";
            var durationInfo = Duration.IsInfinite ? "" : $" [{Duration}]";
            return $"{Name}{stackInfo}{durationInfo} - {Description}";
        }
    }
}