using Godot;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Screens;

public class SettingsMenuScreen : NodeRepo<ISettingsMenuScreen>, ISettingsMenuScreenRepo
{
    public event Action? AudioButtonPressed;
    
    private void EmitAudioButtonPressed() => AudioButtonPressed?.Invoke();
    public event Action? VideoButtonPressed;
    
    private void EmitVideoButtonPressed() => VideoButtonPressed?.Invoke();
    public event Action? BackButtonPressed;
    
    private void EmitBackButtonPressed() => BackButtonPressed?.Invoke();
    

    public SettingsMenuScreen(ISettingsMenuScreen settingsMenuScreen)
    {
        Node = settingsMenuScreen;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        if (Node == null) return;
        Node.OnAudioButtonPressed += EmitAudioButtonPressed;
        Node.OnVideoButtonPressed += EmitVideoButtonPressed;
        Node.OnBackButtonPressed += EmitBackButtonPressed;
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        if (Node == null) return;
        Node.OnAudioButtonPressed -= EmitAudioButtonPressed;
        Node.OnVideoButtonPressed -= EmitVideoButtonPressed;
        Node.OnBackButtonPressed -= EmitBackButtonPressed;
    }
}