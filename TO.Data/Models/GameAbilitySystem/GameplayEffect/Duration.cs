namespace TO.Data.Models.GameAbilitySystem.GameplayEffect;

/// <summary>
/// 持续时间值对象
/// </summary>
public class Duration
{
    /// <summary>
    /// 是否为无限持续时间
    /// </summary>
    public bool IsInfinite { get; }
        
    /// <summary>
    /// 总时间
    /// </summary>
    public float TotalTime { get; }
        
    /// <summary>
    /// 剩余时间
    /// </summary>
    public float RemainingTime { get; private set; }
        
    /// <summary>
    /// 私有构造函数
    /// </summary>
    /// <param name="isInfinite">是否无限</param>
    /// <param name="totalTime">总时间</param>
    public Duration(bool isInfinite, float totalTime)
    {
        IsInfinite = isInfinite;
        TotalTime = totalTime;
        RemainingTime = TotalTime;
    }
        
    /// <summary>
    /// 创建无限持续时间
    /// </summary>
    public static Duration? Infinite => new(true, 0);
        
    /// <summary>
    /// 创建有限持续时间
    /// </summary>
    /// <param name="seconds">持续秒数</param>
    /// <returns>持续时间对象</returns>
    public static Duration FromSeconds(float seconds)
    {
        if (seconds <= 0)
            throw new ArgumentException("Duration must be positive", nameof(seconds));
                
        return new Duration(false, seconds);
    }
        
    /// <summary>
    /// 检查是否已过期
    /// </summary>
    public bool IsExpired => !IsInfinite && RemainingTime <= 0;
        
    /// <summary>
    /// 获取剩余百分比
    /// </summary>
    public float RemainingPercentage => IsInfinite ? 1.0f : Math.Max(0, RemainingTime / TotalTime);
        
    /// <summary>
    /// 更新持续时间
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    public void Update(float deltaTime)
    {
        if (!IsInfinite)
        {
            RemainingTime = Math.Max(0, RemainingTime - deltaTime);
        }
    }
        
    /// <summary>
    /// 重置持续时间
    /// </summary>
    public void Reset()
    {
        if (!IsInfinite)
        {
            RemainingTime = TotalTime;
        }
    }
        
    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    /// <returns>用于比较的组件</returns>
    protected virtual IEnumerable<object> GetEqualityComponents()
    {
        yield return IsInfinite;
        yield return TotalTime;
        yield return RemainingTime;
    }
        
    /// <summary>
    /// 判断两个持续时间是否相等
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>是否相等</returns>
    public override bool Equals(object obj)
    {
        if (obj is Duration other)
        {
            return IsInfinite == other.IsInfinite &&
                   Math.Abs(TotalTime - other.TotalTime) < float.Epsilon &&
                   Math.Abs(RemainingTime - other.RemainingTime) < float.Epsilon;
        }
        return false;
    }
        
    /// <summary>
    /// 获取哈希码
    /// </summary>
    /// <returns>哈希码</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(IsInfinite, TotalTime, RemainingTime);
    }
        
    /// <summary>
    /// 获取字符串表示
    /// </summary>
    /// <returns>格式化的持续时间字符串</returns>
    public override string ToString()
    {
        if (IsInfinite)
            return "Infinite";
        return $"{RemainingTime:F1}s / {TotalTime:F1}s";
    }
}