using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;


namespace TO.Apps.Events;

public record ShowUI(UIName UIName) : IEvent;

public record Hide() : IEvent;

public record HideAll() : IEvent;