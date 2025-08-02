using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Bases;

namespace TO.Services.Core.GameAbilitySystem.Base;

/// <summary>
/// GameAbilitySystem服务基类
/// 提供事件总线支持和通用功能
/// </summary>
public abstract class BaseGameAbilityService : BaseService
{
    protected readonly IEventBusRepo EventBus;
    protected readonly object _lock = new object();
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventBus">事件总线</param>
    protected BaseGameAbilityService(IEventBusRepo eventBus)
    {
        EventBus = eventBus;
    }
    
    /// <summary>
    /// 安全发布事件到事件总线
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    protected void PublishEvent<T>(T @event) where T : IEvent
    {
        try
        {
            EventBus.Publish(@event);
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常，避免影响业务逻辑
            Console.WriteLine($"发布事件失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 执行带锁的操作
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="action">要执行的操作</param>
    /// <returns>操作结果</returns>
    protected T ExecuteWithLock<T>(Func<T> action)
    {
        lock (_lock)
        {
            return action();
        }
    }
    
    /// <summary>
    /// 执行带锁的操作（无返回值）
    /// </summary>
    /// <param name="action">要执行的操作</param>
    protected void ExecuteWithLock(Action action)
    {
        lock (_lock)
        {
            action();
        }
    }
    
}