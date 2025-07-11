namespace TO.Commons.Enums.Infrastructure;
// 资源缓存策略
public enum CachePolicy
{
    Normal,     // 默认策略
    Persistent, // 常驻内存
    Temporary   // 优先释放
}