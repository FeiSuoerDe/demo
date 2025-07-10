using TO.Commons.Enums;
using TO.Repositories.Abstractions.Core.EventBus;

namespace TO.Events.Core;

public record ShowUI(UIName UIName) : IEvent;

public record HideUI() : IEvent;

public record HideAllUI() : IEvent;

public record CloseUI(UIName UIName) : IEvent;

public record CloseAllUI() : IEvent;