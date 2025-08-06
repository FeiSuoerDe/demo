using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayEffect;

/// <summary>
/// 属性效果类，表示一个可以影响属性的效果
/// 根据GAS设计文档要求，支持完整的效果管理功能
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
    public List<AttributeModifier?> Modifiers { get; private set; }
        
    /// <summary>
    /// 持续时间
    /// </summary>
    public Duration Duration { get; private set; }

    /// <summary>
    /// 效果类型
    /// </summary>
    public EffectType EffectType { get; }
        
    /// <summary>
    /// 效果标签
    /// </summary>
    public HashSet<EffectTags> Tags { get; private set; }
        
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
    /// 效果优先级
    /// </summary>
    public int Priority { get; private set; }
        
        
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; private set; }
        
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdatedTime { get; private set; }
        
    /// <summary>
    /// 是否为被动效果
    /// </summary>
    public bool IsPassive { get; private set; }
        
    /// <summary>
    /// 效果状态
    /// </summary>
    public EffectStatus Status { get; private set; }
        
    /// <summary>
    /// 是否为周期性效果
    /// </summary>
    public bool IsPeriodic { get; private set; }
        
    /// <summary>
    /// 周期执行间隔（秒）
    /// </summary>
    public double IntervalSeconds { get; private set; }

    /// <summary>
    /// 效果来源
    /// </summary>
    public GameplayEffectSource? Source { get; private set; }
        
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
    /// <param name="priority">效果优先级</param>
    /// <param name="isPassive">是否为被动效果</param>
    /// <param name="isPeriodic">是否为周期性效果</param>
    /// <param name="intervalSeconds">周期执行间隔（秒）</param>
    public AttributeEffect(string name, string description, List<AttributeModifier?> modifiers, 
        Duration duration, EffectType effectType, HashSet<EffectTags> tags, 
        EffectStackingType stackingType = EffectStackingType.Replace, int maxStacks = 1,
        int priority = 0, bool isPassive = false, bool isPeriodic = false, double intervalSeconds = 1.0)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Modifiers = modifiers;
        Duration = duration;
        EffectType = effectType;
        Tags = tags;
        StackingType = stackingType;
        MaxStacks = maxStacks;
        CurrentStacks = 1; // 初始为1层
        Priority = priority;
        
        IsPassive = isPassive;
        IsPeriodic = isPeriodic;
        IntervalSeconds = intervalSeconds;
        Status = EffectStatus.Active;
        CreatedTime = DateTime.UtcNow;
        LastUpdatedTime = CreatedTime;
    }
        
    /// <summary>
    /// 检查效果是否已过期
    /// </summary>
    public bool IsExpired => Duration.IsExpired && Status == EffectStatus.Active;
        
    /// <summary>
    /// 是否为永久效果
    /// </summary>
    public bool IsPermanent => Duration.IsInfinite;
        
    /// <summary>
    /// 是否为临时效果
    /// </summary>
    public bool IsTemporary => !Duration.IsInfinite;
        
    /// <summary>
    /// 获取剩余时间百分比
    /// </summary>
    public float RemainingTimePercentage => Duration.RemainingPercentage;
        
    /// <summary>
    /// 更新持续时间
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    public void UpdateDuration(float deltaTime)
    {
        if (Status == EffectStatus.Active && !Duration.IsInfinite)
        {
            Duration.Update(deltaTime);
            LastUpdatedTime = DateTime.UtcNow;
                
            if (Duration.IsExpired)
            {
                Status = EffectStatus.Expired;
            }
        }
            
    }
        
    /// <summary>
    /// 暂停效果
    /// </summary>
    public void Pause()
    {
        if (Status == EffectStatus.Active)
        {
            Status = EffectStatus.Paused;
            LastUpdatedTime = DateTime.UtcNow;
        }
    }
        
    /// <summary>
    /// 恢复效果
    /// </summary>
    public void Resume()
    {
        if (Status == EffectStatus.Paused)
        {
            Status = EffectStatus.Active;
            LastUpdatedTime = DateTime.UtcNow;
        }
    }
        
    /// <summary>
    /// 取消效果
    /// </summary>
    public void Cancel()
    {
        Status = EffectStatus.Cancelled;
        LastUpdatedTime = DateTime.UtcNow;
    }
        
    /// <summary>
    /// 添加层数
    /// </summary>
    /// <param name="count">堆叠数量</param>
    /// <returns>是否成功添加</returns>
    public bool AddStack(int count = 1)
    {
        if (StackingType == EffectStackingType.NoStack || Status != EffectStatus.Active)
            return false;
                
        var newStackCount = CurrentStacks + count;
        if (newStackCount <= MaxStacks)
        {
            CurrentStacks = newStackCount;
            LastUpdatedTime = DateTime.UtcNow;
            return true;
        }
            
        CurrentStacks = MaxStacks;
        LastUpdatedTime = DateTime.UtcNow;
        return false;
    }
        
    /// <summary>
    /// 移除层数
    /// </summary>
    /// <param name="count">移除数量</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveStack(int count = 1)
    {
        if (StackingType == EffectStackingType.NoStack || Status != EffectStatus.Active)
            return false;
                
        var newStackCount = CurrentStacks - count;
        if (newStackCount > 0)
        {
            CurrentStacks = newStackCount;
            LastUpdatedTime = DateTime.UtcNow;
            return true;
        }
            
        CurrentStacks = 0;
        Status = EffectStatus.Expired;
        LastUpdatedTime = DateTime.UtcNow;
        return false;
    }
        
    /// <summary>
    /// 重置持续时间
    /// </summary>
    public void RefreshDuration()
    {
        if (Status == EffectStatus.Active && !Duration.IsInfinite)
        {
            Duration.Reset();
            LastUpdatedTime = DateTime.UtcNow;
        }
    }
        
    /// <summary>
    /// 设置新的持续时间
    /// </summary>
    /// <param name="newDuration">新的持续时间</param>
    public void SetDuration(Duration newDuration)
    {
        Duration = newDuration;
        LastUpdatedTime = DateTime.UtcNow;
            
        if (Duration.IsExpired)
        {
            Status = EffectStatus.Expired;
        }
    }

    public void SetSource(GameplayEffectSource? source)
    {
        Source = source;
    }
    /// <summary>
    /// 添加修饰器
    /// </summary>
    /// <param name="modifier">要添加的修饰器</param>
    public void AddModifier(AttributeModifier? modifier)
    {
        if (modifier == null)
            throw new ArgumentNullException(nameof(modifier));
                
        Modifiers.Add(modifier);
        LastUpdatedTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 添加标签
    /// </summary>
    /// <param name="tag">要添加的标签</param>
    public void AddTag(EffectTags tag)
    {

        Tags.Add(tag);
        LastUpdatedTime = DateTime.UtcNow;

    }

    /// <summary>
    /// 移除标签
    /// </summary>
    /// <param name="tag">要移除的标签</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveTag(EffectTags tag)
    {
        if (Tags.Remove(tag))
        {
            LastUpdatedTime = DateTime.UtcNow;
            return true;
        }
        return false;
    }
        
    /// <summary>
    /// 检查是否包含指定标签
    /// </summary>
    /// <param name="tag">要检查的标签</param>
    /// <returns>是否包含该标签</returns>
    public bool HasTag(EffectTags tag)
    {
        return Tags.Contains(tag);
    }
        
    /// <summary>
    /// 检查是否包含任意指定标签
    /// </summary>
    /// <param name="tags">要检查的标签集合</param>
    /// <returns>是否包含任意标签</returns>
    public bool HasAnyTag(IEnumerable<EffectTags> tags)
    {
        return tags.Any(tag => Tags.Contains(tag));
    }
        
    /// <summary>
    /// 检查是否包含所有指定标签
    /// </summary>
    /// <param name="tags">要检查的标签集合</param>
    /// <returns>是否包含所有标签</returns>
    public bool HasAllTags(IEnumerable<EffectTags> tags)
    {
        return tags.All(tag => Tags.Contains(tag));
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
    public IEnumerable<AttributeModifier?> GetModifiersForAttribute(AttributeDefinition attributeType)
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

    public AttributeEffect Clone()
    {
        return new AttributeEffect(Name, Description, Modifiers.Select(m => m.Clone()).ToList(), 
            Duration, EffectType, Tags,StackingType, MaxStacks, Priority, IsPassive);
    }
}
