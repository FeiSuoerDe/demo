using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;


namespace TO.Apps.Events;

public record ShowUI(UIName UIName) : IEvent;

public record HideUI() : IEvent;

public record HideAllUI() : IEvent;

public record CloseUI(UIName UIName) : IEvent;

public record CloseAllUI() : IEvent;