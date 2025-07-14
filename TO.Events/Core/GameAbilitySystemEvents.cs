using TO.Data.Models.GameAbilitySystem.GameplayAbility;
using TO.Data.Models.GameAbilitySystem.GameplayEffect;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Commons.Enums.Game;

namespace TO.Events.Core;

// 技能冷却相关事件
public record CooldownStarted(string PlayerId, string AbilityId, TimeSpan Duration) : IEvent;
public record CooldownEnded(string PlayerId, string AbilityId) : IEvent;
public record CooldownReset(string PlayerId, string AbilityId) : IEvent;
public record GlobalCooldownStarted(string PlayerId, TimeSpan Duration) : IEvent;

// 技能预设相关事件
public record PresetsLoaded(int Count) : IEvent;
public record PresetCreated(string PresetId, object Preset) : IEvent;
public record PresetUpdated(string PresetId, object Preset) : IEvent;
public record PresetDeleted(string PresetId) : IEvent;

// AI决策相关事件
public record AIDecisionMade(string AiId, string AbilityId, object DecisionContext) : IEvent;
public record AIStrategyUpdated(string AiId, object BehaviorProfile) : IEvent;

// 技能分析相关事件
public record AbilityDataUpdated(string AbilityId, object UsageData) : IEvent;
public record AnalysisCompleted(object AnalysisReport) : IEvent;
public record AnomalyDetected(string AbilityId, string AnomalyType, string Description) : IEvent;

// 技能管理相关事件
public record AbilityActivated(Guid OwnerId, GameplayAbility Ability, object ActivationResult) : IEvent;
public record AbilityCooldownCompleted(Guid OwnerId, GameplayAbility Ability) : IEvent;
public record AbilityLeveledUp(Guid OwnerId, GameplayAbility Ability, int OldLevel) : IEvent;

// 技能升级相关事件
public record AbilityLevelUp(string AbilityId, int CurrentLevel, int NextLevel) : IEvent;
public record AbilityExperienceGained(string AbilityId, int Experience, int CurrentExperience) : IEvent;
public record AbilityUnlocked(string AbilityId) : IEvent;

// 技能效果相关事件
public record EffectApplied(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRemoved(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectRefreshed(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;
public record EffectExpired(string TargetId, AttributeEffect Effect, AttributeSet Target) : IEvent;

// 技能系统管理相关事件
public record AbilitySystemActivated(string AbilityId, GameplayAbility Ability) : IEvent;
public record AbilitySystemCompleted(string AbilityId, GameplayAbility Ability) : IEvent;
public record AbilitySystemCancelled(string AbilityId, GameplayAbility Ability) : IEvent;
public record SystemEffectApplied(string TargetId, AttributeEffect Effect) : IEvent;
public record SystemEffectRemoved(string TargetId, AttributeEffect Effect) : IEvent;

// 技能配置相关事件
public record ConfigChanged(string AbilityId, object Config) : IEvent;
public record ConfigsLoaded(int Count) : IEvent;

// 属性管理相关事件
public record AttributeChanged(Guid AttributeSetId, AttributeType AttributeType, float OldValue, float NewValue) : IEvent;
public record AttributeEffectApplied(Guid AttributeSetId, object AttributeEffect) : IEvent;
public record AttributeEffectRemoved(Guid AttributeSetId, object AttributeEffect) : IEvent;

// 技能连招相关事件
public record ComboStarted(string PlayerId, object ComboDefinition) : IEvent;
public record ComboCompleted(string PlayerId, object ComboDefinition, bool Success) : IEvent;
public record ComboInterrupted(string PlayerId, object ComboDefinition, int Step) : IEvent;
public record AbilityChainExecuted(string PlayerId, string AbilityId, int Step) : IEvent;