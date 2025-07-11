using TO.Commons.Enums.UI;

namespace TO.Commons.Configs;

public class UIConfigs
{
    public static Dictionary<UIName,string> UIPaths = new()
    {
        {UIName.MainMenuScreen, "res://Scenes/UI/Screens/MainMenuScreen.tscn"},
        {UIName.SettingsMenuScreen, "res://Scenes/UI/Screens/SettingsMenuScreen.tscn"},
        {UIName.VolumeSettingsScreen, "res://Scenes/UI/Screens/VolumeSettingsScreen.tscn"},
        {UIName.LoadingScreen, "res://Scenes/UI/Screens/LoadingScreen.tscn"}
    };
}