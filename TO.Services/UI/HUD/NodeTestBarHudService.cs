using Godot;
using GodotTask;
using TO.Commons.Enums.Game;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Bases;

namespace TO.Services.UI.HUD;

public class NodeTestBarHudService : BaseService
{
    private Slider _hSlider;
    private Label _hLabel;
    
    private readonly ITestBarHud _testBarHud;
    private readonly IEventBusRepo _eventBusRepo;
    private readonly IAttributeManagerService _attributeManagerService;
    
    private Guid _modelId;
    private float _maxHealth;
    private float _minHealth;

    public NodeTestBarHudService(ITestBarHud testBarHud, IEventBusRepo eventBusRepo, IAttributeManagerService attributeManagerService)
    {
        _testBarHud = testBarHud;
        _eventBusRepo = eventBusRepo;
        _attributeManagerService = attributeManagerService;
        _hSlider = _testBarHud.HSlider;
        _hLabel = _testBarHud.HLabel;
        _testBarHud.BindModel += BindModel;
    }

    private void BindModel(Guid id)
    {
        _modelId = id;
        _eventBusRepo.Subscribe<AttributeChanged>(OnAttributeChanged).AddTo(CancellationTokenSource.Token);
        _eventBusRepo.Subscribe<AttributeRangeChanged>(OnAttributeRangeChanged).AddTo(CancellationTokenSource.Token);
        var health = _attributeManagerService.GetAttributeValue(_modelId, AttributeType.Health);
        if (health == null) return;
        _maxHealth = health.MaxValue;
        _minHealth = health.MinValue;
        _hLabel.Text = $"{health.CurrentValue}/ {_maxHealth}";
        _hSlider.Value = health.CurrentValue / _maxHealth;
    }

    private void OnAttributeChanged(AttributeChanged @event)
    {
        if (@event.AttributeSetId != _modelId) return;
        if (@event.AttributeType != AttributeType.Health) return;
        GD.Print($"OnAttributeChanged: {@event.NewValue}/ {_maxHealth}");
        _hLabel.Text = $"{@event.NewValue}/ {_maxHealth}";
        _hSlider.Value = @event.NewValue / _maxHealth;

    }
    
    private void OnAttributeRangeChanged(AttributeRangeChanged @event)
    {
        GD.Print($"OnAttributeRangeChanged: {nameof(@event)}");
        if (@event.AttributeSetId != _modelId) return;
        if (@event.AttributeType != AttributeType.Health) return;
        var oldValue = _maxHealth;
        _maxHealth = @event.MaxValue;
        _minHealth = @event.MinValue;
        var replace = _hLabel.Text.Replace($"{_maxHealth}", $"/{oldValue}");
        _hLabel.Text = replace;
    }
    
    protected override void UnSubscriber()
    {
        _testBarHud.BindModel -= BindModel;
    }
}