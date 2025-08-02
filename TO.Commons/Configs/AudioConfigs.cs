using TO.Commons.Enums.System;

namespace TO.Commons.Configs;

public static class AudioConfigs
{
    
    public static Dictionary<AudioEnums.Audio, string> AudioPath = new()
    {
        {  AudioEnums.Audio.Button, "res://Assets/Sounds/SFX/Classic Status Effects/Dispelled.wav"}
    };
}