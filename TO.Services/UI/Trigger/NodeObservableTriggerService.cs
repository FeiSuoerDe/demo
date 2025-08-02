using Godot;
using GodotTask;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.UI.Bases;
using TO.Nodes.Abstractions.UI.Trigger;
using TO.Services.Bases;

namespace TO.Services.UI.Trigger;

public class NodeObservableTriggerService : BaseService
{
    private readonly IObservableTrigger _observableTrigger;
    private readonly List<Action> _unregisterActions = [];
    
    public NodeObservableTriggerService(IObservableTrigger observableTrigger)
    {
        _observableTrigger = observableTrigger;
        _ = InitializeAsync();
        Register();
    }
    
    private async Task InitializeAsync()
    {
        try
        {
            var sceneTree = (SceneTree)Engine.GetMainLoop();
            if (sceneTree.Root.IsNodeReady() || !_observableTrigger.TriggerOnReady) 
                return;
            
            await sceneTree.ToSignal(sceneTree, Godot.Node.SignalName.Ready).AsGDTask().Timeout(TimeSpan.FromSeconds(10));
            _observableTrigger.RiseTrigger();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Initialize failed: {e}");
            throw new InvalidOperationException("Node observable trigger initialization failed", e);
        }
    }
    
    private void Register()
    {
        if (_observableTrigger.TriggerControl == null)
            return;

        switch (_observableTrigger.TriggerType)
        {
            case TriggerType.Pressed:
                RegisterGuiInput(e => 
                {
                    if (e is InputEventMouseButton mouseButton && mouseButton.IsPressed())
                        _observableTrigger.RiseTrigger();
                    e.Dispose();
                });
                break;
                
            case TriggerType.Released:
                RegisterGuiInput(e => 
                {
                    if (e is InputEventMouseButton mouseButton && mouseButton.IsReleased())
                        _observableTrigger.RiseTrigger();
                    e.Dispose();
                });
                break;
                
            case TriggerType.MouseIn:
                RegisterMouseEvent(_observableTrigger.TriggerControl, 
                    () => _observableTrigger.RiseTrigger(), 
                    isEnterEvent: true);
                break;
                
            case TriggerType.MouseOut:
                RegisterMouseEvent(_observableTrigger.TriggerControl, 
                    () => _observableTrigger.RiseTrigger(), 
                    isEnterEvent: false);
                break;
                
            case TriggerType.MouseInOut:
                RegisterMouseEvent(_observableTrigger.TriggerControl, 
                    () => _observableTrigger.RiseTrigger(new Dictionary<string, object> { ["isEntered"] = true }), 
                    isEnterEvent: true);
                RegisterMouseEvent(_observableTrigger.TriggerControl, 
                    () => _observableTrigger.RiseTrigger(new Dictionary<string, object> { ["isEntered"] = false }), 
                    isEnterEvent: false);
                break;
                
            case TriggerType.PressedReleased:
                RegisterGuiInput(e => 
                {
                    if (e is InputEventMouseButton mouseButton1 && mouseButton1.IsPressed())
                        _observableTrigger.RiseTrigger(new Dictionary<string, object> { ["isPressed"] = true });
                    if (e is InputEventMouseButton mouseButton && mouseButton.IsReleased())
                        _observableTrigger.RiseTrigger(new Dictionary<string, object> { ["isPressed"] = false });
                    e.Dispose();
                });
                break;
                
            case TriggerType.DoubleClicked:
                RegisterGuiInput(e => 
                {
                    if (e is InputEventMouseButton { DoubleClick: true })
                        _observableTrigger.RiseTrigger();
                    e.Dispose();
                });
                break;
                
            case TriggerType.Ready:
                RegisterReadyEvent(_observableTrigger.TriggerControl, 
                    () => _observableTrigger.RiseTrigger());
                break;
                
            case TriggerType.VisibilityChanged:
                if (_observableTrigger.TriggerControl is IUIScreen screen)
                    RegisterVisibilityChangedEvent(screen, 
                        visible => _observableTrigger.RiseTrigger(new Dictionary<string, object> { ["Visible"] = visible }));
                break;
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RegisterGuiInput(Control.GuiInputEventHandler handler)
    {
        if (_observableTrigger.TriggerControl == null)
        {
            GD.PrintErr("Cannot register GuiInput event - control is null");
            return;
        }

        _observableTrigger.TriggerControl.GuiInput += handler;
        _unregisterActions.Add(() => 
        {
            if (_observableTrigger.TriggerControl != null)
                _observableTrigger.TriggerControl.GuiInput -= handler;
        });
    }

    private void RegisterMouseEvent(Control? control, Action handler, bool isEnterEvent = true)
    {
        if (control == null)
        {
            GD.PrintErr("Cannot register mouse event - control is null");
            return;
        }

        try
        {
            if (isEnterEvent)
            {
                control.MouseEntered += handler;
                _unregisterActions.Add(() => { control.MouseEntered -= handler; });
            }
            else
            {
                control.MouseExited += handler;
                _unregisterActions.Add(() => { control.MouseExited -= handler; });
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to register mouse event: {e}");
        }
    }

    private void RegisterReadyEvent(Control? control, Action handler)
    {
        if (control == null)
        {
            GD.PrintErr("Cannot register Ready event - control is null");
            return;
        }

        control.Ready += handler;
        _unregisterActions.Add(() => { control.Ready -= handler; });
    }

    private void RegisterVisibilityChangedEvent(IUIScreen? screen, Action<bool> handler)
    {
        if (screen == null)
        {
            GD.PrintErr("Cannot register VisibilityChanged event - screen is null");
            return;
        }

        screen.OnVisibilityChanged += handler;
        _unregisterActions.Add(() => { screen.OnVisibilityChanged -= handler; });
    }
    

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing) return;
        foreach (var action in _unregisterActions)
        {
            try 
            { 
                action?.Invoke(); 
            }
            catch (Exception e) 
            { 
                GD.PrintErr($"Unregister failed: {e}"); 
            }
        }
        _unregisterActions.Clear();

    }
}