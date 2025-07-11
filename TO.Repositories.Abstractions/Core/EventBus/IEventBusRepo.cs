namespace TO.Repositories.Abstractions.Core.EventBus;

public interface IEventBusRepo
{
    void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
}