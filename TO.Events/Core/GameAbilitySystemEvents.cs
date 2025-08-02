using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Repositories.Abstractions.Core.EventBus;

namespace TO.Events.Core;



// 技能效果相关事件
public record EffectApplied(Guid TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRemoved(Guid TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRefreshed(Guid TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectExpired(Guid TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;

// 属性管理相关事件
public record AttributeChanged(Guid AttributeSetId, AttributeDefinition AttributeType, float OldValue, float NewValue) : IEvent;
public record AttributeRangeChanged(Guid AttributeSetId, AttributeDefinition AttributeType, float MinValue, float MaxValue) : IEvent;
