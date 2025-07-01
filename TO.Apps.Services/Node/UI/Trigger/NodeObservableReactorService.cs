using Godot;
using TO.Apps.Services.Abstractions.Bases;
using TO.Domains.Models.Repositories.Abstractions.Core.LogSystem;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Trigger;
using TO.Domains.Services.Abstractions.Core.UISystem;
using TO.Nodes.Abstractions.Nodes.UI.Trigger;

namespace TO.Apps.Services.Node.UI.Trigger;

public class NodeObservableReactorService : BaseService
{
    
    private readonly IObservableReactorRepo _observableReactorRepo;
    private readonly IUiAnimationService _uiAnimationService;
    private readonly ILoggerRepo _loggerRepo;

    private readonly Dictionary<string, Action<Control?, 
        Tween.EaseType, Tween.TransitionType,
        Godot.Collections.Array<Variant>?, 
        Dictionary<string, object>?>> _animations;
    
    public NodeObservableReactorService(IObservableReactorRepo observableReactorRepo, IUiAnimationService uiAnimationService, ILoggerRepo loggerRepo)
    {
        _observableReactorRepo = observableReactorRepo;
        _uiAnimationService = uiAnimationService;
        _loggerRepo = loggerRepo;
        _animations = new Dictionary<string, Action<Control?, 
            Tween.EaseType, Tween.TransitionType, 
            Godot.Collections.Array<Variant>?, 
            Dictionary<string, object>?>>
        {
            { "MouseInOut", _uiAnimationService.MouseInOut },
            { "MousePressedReleased", _uiAnimationService.MousePressedReleased },
            { "Popup", _uiAnimationService.Popup }
        };
        
        if (_observableReactorRepo.Trigger == null) return;
        
        _observableReactorRepo.Trigger.Triggered += Triggered;

    }

    private void Triggered(Dictionary<string, object>? data)
    {
        if (_animations.TryGetValue(_observableReactorRepo.FnName, out var action))
        {
            action(_observableReactorRepo.ReactControl, 
                _observableReactorRepo.Ease, 
                _observableReactorRepo.Trans, 
                _observableReactorRepo.FnArgs, 
                data);
        }

    }


    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        if (_observableReactorRepo.Trigger == null) return;
        
        _observableReactorRepo.Trigger.Triggered -= Triggered;
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _animations.Clear();
    }
}