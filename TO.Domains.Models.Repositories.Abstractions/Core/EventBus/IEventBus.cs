namespace TO.Domains.Models.Repositories.Abstractions.Core.EventBus;

public interface IEventBus
{
    void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
}