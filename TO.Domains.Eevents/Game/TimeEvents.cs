using Infras.Writers;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;

namespace TO.Domains.Eevents.Game;

/// <summary>
/// 时间更新事件
/// </summary>
/// <param name="Time">游戏时间</param>
public record OnTimeUpdated(GameTime Time) : IEvent;

/// <summary>
/// 季节变化事件
/// </summary>
/// <param name="Season">新的季节</param>
public record OnSeasonChanged(Season Season) : IEvent;