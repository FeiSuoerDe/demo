using Godot;
using GodotTask;
using TO.Data.Attributes;
using TO.Data.Registries;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.HUD;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Abstractions.Core.GameAbilitySystem.GameplayAttribute;
using TO.Services.Bases;

namespace TO.Services.UI.HUD;

public class NodeTestBarHudService : BaseService
{
    private Slider _hSlider;
    private Label _hLabel;

    private Label _conLabel;

    private string _gameAttributes;

    private string _gameAttributes_2;
    
    private readonly ITestBarHud _testBarHud;
    private readonly IEventBusRepo _eventBusRepo;
    private readonly IAttributeManagerService _attributeManagerService;
    
    private Guid _modelId;

    public NodeTestBarHudService(ITestBarHud testBarHud, IEventBusRepo eventBusRepo, IAttributeManagerService attributeManagerService)
    {
        _testBarHud = testBarHud;
        _eventBusRepo = eventBusRepo;
        _attributeManagerService = attributeManagerService;
        _hSlider = _testBarHud.HSlider;
        _hLabel = _testBarHud.HLabel;
        _conLabel = testBarHud.ConLabel;
        _gameAttributes = testBarHud.gameAttributes;
        _gameAttributes_2 = testBarHud.gameAttributes_2;
        _testBarHud.BindModel += BindModel;
    }

    private void BindModel(Guid id)
    {
        _modelId = id;
        _eventBusRepo.Subscribe<AttributeChanged>(OnAttributeChanged).AddTo(CancellationTokenSource.Token);

        var health = _attributeManagerService.GetAttributeValue(_modelId, AttributeRegistry.Get(_gameAttributes));
        var maxHealth = _attributeManagerService.GetAttributeValue(_modelId, AttributeRegistry.Get(_gameAttributes_2));
        var constitution = _attributeManagerService.GetAttributeValue(_modelId, GameAttributes.Constitution);
        if (health == null || maxHealth == null) return;
        _hLabel.Text = $"{health.CurrentValue}/ {maxHealth.CurrentValue}";
        _hSlider.Value = health.CurrentValue / maxHealth.CurrentValue;
        if (constitution != null)
        {
            _conLabel.Text = $"Constitution: {constitution.CurrentValue}";
        }
    
    }

    private void OnAttributeChanged(AttributeChanged @event)
    {
        if (@event.AttributeSetId != _modelId) return;
        var health = _attributeManagerService.GetAttributeValue(_modelId, AttributeRegistry.Get(_gameAttributes));
        var maxHealth = _attributeManagerService.GetAttributeValue(_modelId, AttributeRegistry.Get(_gameAttributes_2));
        var constitution = _attributeManagerService.GetAttributeValue(_modelId, GameAttributes.Constitution);
        if (health == null || maxHealth == null) return;
        GD.Print($"health: {health.CurrentValue}, maxHealth: {maxHealth.CurrentValue}");
        _hLabel.Text = $"{health.CurrentValue}/ {maxHealth.CurrentValue}";
        _hSlider.Value = health.CurrentValue / maxHealth.CurrentValue;
        if (constitution != null)
        {
            _conLabel.Text = $"Constitution: {constitution.CurrentValue}";
        }

    }
    

    
    protected override void UnSubscriber()
    {
        _testBarHud.BindModel -= BindModel;
    }
}
