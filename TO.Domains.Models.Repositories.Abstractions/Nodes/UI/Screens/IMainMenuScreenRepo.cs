namespace TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

public interface IMainMenuScreenRepo 
{
    public event Action? StartButtonPressed;
    public event Action? SettingsButtonPressed;
    public event Action? ExitButtonPressed;
}