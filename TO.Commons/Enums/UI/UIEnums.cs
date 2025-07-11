namespace TO.Commons.Enums.UI;

public enum TriggerType
{
    MouseIn,
    MouseOut,
    MouseInOut,
    Pressed,
    Released,
    PressedReleased,
    DoubleClicked,
    Ready, 
    VisibilityChanged,

}

public enum UILayerType
{
    Background = 0,
    Normal = 100,
    Dialog = 200, 
    Popup = 300,
    Loading = 400, 
    Alert = 500,
    System = 600,
}

public enum UIName
{
    MainMenuScreen,
    SettingsMenuScreen,
    VolumeSettingsScreen,
    LoadingScreen
    
}