using Newtonsoft.Json;

namespace TO.Data.Serialization;

/// <summary>
/// 用户游戏设置数据结构
/// </summary>
[Serializable]
public class UserSettings
{
    [JsonProperty("game")]
    public GameSettings Game { get; set; } = new GameSettings();
        
    [JsonProperty("controls")]
    public ControlSettings Controls { get; set; } = new ControlSettings();
        
    [JsonProperty("audio")]
    public AudioSettings Audio { get; set; } = new AudioSettings();
        
    [JsonProperty("ui")]
    public UISettings UI { get; set; } = new UISettings();

    [Serializable]
    public class GameSettings
    {
        [JsonProperty("language")]
        public string Language { get; set; } = "zh_CN";
            
        [JsonProperty("difficulty")]
        public string Difficulty { get; set; } = "normal";
            
        [JsonProperty("autosave_interval")]
        public int AutosaveInterval { get; set; } = 300;
            
        [JsonProperty("show_tutorials")]
        public bool ShowTutorials { get; set; } = true;
            
        [JsonProperty("text_speed")]
        public string TextSpeed { get; set; } = "medium";
    }

    [Serializable]
    public class ControlSettings
    {
        [JsonProperty("keyboard")]
        public KeyboardControls Keyboard { get; set; } = new KeyboardControls();
            
        [JsonProperty("gamepad")]
        public GamepadControls Gamepad { get; set; } = new GamepadControls();

        [Serializable]
        public class KeyboardControls
        {
            [JsonProperty("move_up")]
            public string MoveUp { get; set; } = "W";
                
            [JsonProperty("move_down")]
            public string MoveDown { get; set; } = "S";
                
            [JsonProperty("move_left")]
            public string MoveLeft { get; set; } = "A";
                
            [JsonProperty("move_right")]
            public string MoveRight { get; set; } = "D";
                
            [JsonProperty("interact")]
            public string Interact { get; set; } = "E";
                
            [JsonProperty("inventory")]
            public string Inventory { get; set; } = "I";
        }

        [Serializable]
        public class GamepadControls
        {
            [JsonProperty("vibration_enabled")]
            public bool VibrationEnabled { get; set; } = true;
                
            [JsonProperty("left_stick_deadzone")]
            public float LeftStickDeadzone { get; set; } = 0.2f;
                
            [JsonProperty("right_stick_deadzone")]
            public float RightStickDeadzone { get; set; } = 0.2f;
        }
    }

    [Serializable]
    public class AudioSettings
    {
        [JsonProperty("master_volume")]
        public float MasterVolume { get; set; } = 1;
            
        [JsonProperty("music_volume")]
        public float MusicVolume { get; set; } = 1;
            
        [JsonProperty("sfx_volume")]
        public float SfxVolume { get; set; } = 1;
            
        [JsonProperty("ambience_volume")]
        public float AmbienceVolume { get; set; } = 1;
            
        [JsonProperty("voice_volume")]
        public float VoiceVolume { get; set; } = 1;
            
        [JsonProperty("mute")]
        public bool Mute { get; set; }
    }

    [Serializable]
    public class UISettings
    {
        [JsonProperty("theme")]
        public string Theme { get; set; } = "default";
            
        [JsonProperty("font_size")]
        public string FontSize { get; set; } = "medium";
            
        [JsonProperty("hud_scale")]
        public float HudScale { get; set; } = 1.0f;
            
        [JsonProperty("show_minimap")]
        public bool ShowMinimap { get; set; } = true;
            
        [JsonProperty("show_hud")]
        public bool ShowHud { get; set; } = true;
            
        [JsonProperty("show_damage_numbers")]
        public bool ShowDamageNumbers { get; set; } = true;
    }
}