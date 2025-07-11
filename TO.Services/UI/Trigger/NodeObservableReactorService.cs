using Godot;
using TO.Nodes.Abstractions.UI.Trigger;
using TO.Repositories.Abstractions.Core.LogSystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Services.UI.Trigger;

public class NodeObservableReactorService : BaseService
{
    
    private readonly IObservableReactor _observableReactor;
    private readonly IUiAnimationService _uiAnimationService;
    private readonly ILoggerRepo _loggerRepo;

    private readonly Dictionary<string, Action<Control?, 
        Tween.EaseType, Tween.TransitionType,
        Godot.Collections.Array<Variant>?, 
        Dictionary<string, object>?>> _animations;
    
    public NodeObservableReactorService(IObservableReactor observableReactor, IUiAnimationService uiAnimationService, ILoggerRepo loggerRepo)
    {
        _observableReactor = observableReactor;
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
        
        if (_observableReactor.Trigger == null) return;
        
        _observableReactor.Trigger.Triggered += Triggered;

    }

    private void Triggered(Dictionary<string, object>? data)
    {
        if (_animations.TryGetValue(_observableReactor.FnName, out var action))
        {
            action(_observableReactor.ReactControl, 
                _observableReactor.Ease, 
                _observableReactor.Trans, 
                _observableReactor.FnArgs, 
                data);
        }

    }


    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        if (_observableReactor.Trigger == null) return;
        
        _observableReactor.Trigger.Triggered -= Triggered;
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _animations.Clear();
    }
}