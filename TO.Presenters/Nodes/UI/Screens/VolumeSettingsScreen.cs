using Godot;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Screens;

public class VolumeSettingsScreen : NodeRepo<IVolumeSettingsScreen>, IVolumeSettingsScreenRepo
{

    public Action<double>? OnMasterVolumeChanged { get; set; }
    private void EmitMasterVolumeChanged(double value) => OnMasterVolumeChanged?.Invoke(value);
    public Action<double>? OnMusicVolumeChanged { get; set; }
    private void EmitMusicVolumeChanged(double value) => OnMusicVolumeChanged?.Invoke(value);
    public Action<double>? OnSFXVolumeChanged { get; set; }

    private void EmitSFXVolumeChanged(double value) => OnSFXVolumeChanged?.Invoke(value);
    public Action<double>? OnVoiceVolumeChanged { get; set; }
    private void EmitVoiceVolumeChanged(double value) => OnVoiceVolumeChanged?.Invoke(value);
    public Action<double>? OnAmbientVolumeChanged { get; set; }
    private void EmitAmbientVolumeChanged(double value) => OnAmbientVolumeChanged?.Invoke(value);

    public Action<bool>? OnMuteStateChanged { get; set; }
    private void EmitMuteStateChanged(bool value) => OnMuteStateChanged?.Invoke(value);

    public Action? OnBackButtonPressed { get; set; }
    private void EmitBackButtonPressed() => OnBackButtonPressed?.Invoke();


    public VolumeSettingsScreen(IVolumeSettingsScreen volumeSettingsScreen)
    {
        Node = volumeSettingsScreen;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }

    protected override void ConnectNodeEvents()
    {
        if (Node == null) return;
        Node.OnMasterVolumeChanged += EmitMasterVolumeChanged;
        Node.OnMusicVolumeChanged += EmitMusicVolumeChanged;
        Node.OnSFXVolumeChanged += EmitSFXVolumeChanged;
        Node.OnVoiceVolumeChanged += EmitVoiceVolumeChanged;
        Node.OnAmbientVolumeChanged += EmitAmbientVolumeChanged;
        Node.OnMuteStateChanged += EmitMuteStateChanged;
        Node.OnBackButtonPressed += EmitBackButtonPressed;
    }


    public void SetMasterVolume(float volume)
    {
        Node?.SetMasterVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        Node?.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        Node?.SetSFXVolume(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        Node?.SetVoiceVolume(volume);
    }

    public void SetAmbientVolume(float volume)
    {
        Node?.SetAmbientVolume(volume);
    }

    public void SetMuteState(bool mute)
    {
        Node?.SetMuteState(mute);
    }

    protected override void DisconnectNodeEvents()
    {
        if (Node == null) return;
        Node.OnMasterVolumeChanged -= EmitMasterVolumeChanged;
        Node.OnMusicVolumeChanged -= EmitMusicVolumeChanged;
        Node.OnSFXVolumeChanged -= EmitSFXVolumeChanged;
        Node.OnVoiceVolumeChanged -= EmitVoiceVolumeChanged;
        Node.OnAmbientVolumeChanged -= EmitAmbientVolumeChanged;
        Node.OnMuteStateChanged -= EmitMuteStateChanged;
        Node.OnBackButtonPressed -= EmitBackButtonPressed;
    }
}