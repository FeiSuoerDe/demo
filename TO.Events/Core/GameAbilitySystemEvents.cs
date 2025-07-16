using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Commons.Enums.Game;

namespace TO.Events.Core;



// 技能效果相关事件
public record EffectApplied(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRemoved(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRefreshed(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectExpired(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;

// 属性管理相关事件
public record AttributeChanged(Guid AttributeSetId, AttributeType AttributeType, float OldValue, float NewValue) : IEvent;

