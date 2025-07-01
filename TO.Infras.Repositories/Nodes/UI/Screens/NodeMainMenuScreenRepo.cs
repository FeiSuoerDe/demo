using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;


namespace inFras.Nodes.UI.Screens;

public class NodeMainMenuScreenRepo : NodeRepo<IMainMenuScreen>, IMainMenuScreenRepo
{
    
    public event Action? StartButtonPressed;
    private void EmitStartButtonPressed() => StartButtonPressed?.Invoke();
    public event Action? SettingsButtonPressed;
    private void EmitSettingsButtonPressed() => SettingsButtonPressed?.Invoke();
    public event Action? ExitButtonPressed;
    private void EmitExitButtonPressed() => ExitButtonPressed?.Invoke();
    
    public NodeMainMenuScreenRepo(IMainMenuScreen mainMenuScreen)
    {
        Node = mainMenuScreen;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        if (Node?.StartButton != null) Node.StartButton.Pressed += EmitStartButtonPressed;
        if (Node?.SettingsButton != null) Node.SettingsButton.Pressed += EmitSettingsButtonPressed;
        if (Node?.ExitButton != null) Node.ExitButton.Pressed += EmitExitButtonPressed;
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        if (Node?.StartButton != null) Node.StartButton.Pressed -= EmitStartButtonPressed;
        if (Node?.SettingsButton != null) Node.SettingsButton.Pressed -= EmitSettingsButtonPressed;
        if (Node?.ExitButton != null) Node.ExitButton.Pressed -= EmitExitButtonPressed;
    }
   
}