using TO.Repositories.Abstractions.Core.EventBus;

namespace TO.Events.Core;

/// <summary>
/// 加载进度事件
/// </summary>
/// <param name="Progress">加载进度(0.0-1.0)</param>
public record LoadingProgress(float Progress) : IEvent;