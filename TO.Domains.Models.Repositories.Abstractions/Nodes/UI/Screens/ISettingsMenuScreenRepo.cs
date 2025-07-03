namespace TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

public interface ISettingsMenuScreenRepo
{
    public event Action? AudioButtonPressed;
    public event Action? VideoButtonPressed;
    public event Action? BackButtonPressed;
}