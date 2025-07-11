using R3;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.EventBus;

public class EventBusRepo: BaseRepo,IEventBusRepo
{
    private readonly Dictionary<Type, Subject<object>> _subjects = new();
    private readonly CompositeDisposable _disposables = new();

    public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_subjects.TryGetValue(eventType, out var subject))
        {
            subject = new Subject<object>();
            _subjects[eventType] = subject;
            _disposables.Add(subject);
        }
        subject.OnNext(@event); // R3事件推送
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_subjects.TryGetValue(eventType, out var subject))
        {
            subject = new Subject<object>();
            _subjects[eventType] = subject;
            _disposables.Add(subject);
        }
        return subject.Subscribe(evt => handler((TEvent)evt)); // R3订阅转换
    }
    
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) {
            _disposables.Dispose();
        }
        // 释放非托管资源（如文件句柄、数据库连接）
        _disposed = true;
    }
}