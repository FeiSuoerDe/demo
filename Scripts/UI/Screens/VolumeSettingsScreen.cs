using System;
using Contexts;
using demo.UI.Components;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens
{
    public partial class VolumeSettingsScreen : Bases.UIScreen, IVolumeSettingsScreen
    {
        [Export] private SliderComponents? MasterSliderComponents { get; set; }

        [Export] private SliderComponents? MusicSliderComponents { get; set; }
        [Export] private SliderComponents? SFXSliderComponents { get; set; }
        [Export] private SliderComponents? VoiceSliderComponents { get; set; }
        [Export] private SliderComponents? AmbientSliderComponents { get; set; }

        [Export] public CheckBox? MuteCheckBox { get; set; }

        [Export] private Button? BackButton { get; set; }

        public Action<double>? OnMasterVolumeChanged { get; set; }
        
        private void EmitOnMasterVolumeChanged(double  value)  => OnMasterVolumeChanged?.Invoke(value);
        public Action<double>? OnMusicVolumeChanged { get; set; }
        
        private void EmitOnMusicVolumeChanged(double  value)  => OnMusicVolumeChanged?.Invoke(value);
        public Action<double>? OnSFXVolumeChanged { get; set; }
        
         private void EmitOnSFXVolumeChanged(double  value)  => OnSFXVolumeChanged?.Invoke(value);
        public Action<double>? OnVoiceVolumeChanged { get; set; }
        
        private void EmitOnVoiceVolumeChanged(double  value)  => OnVoiceVolumeChanged?.Invoke(value);
        public Action<double>? OnAmbientVolumeChanged { get; set; }
        
        private void EmitOnAmbientVolumeChanged(double  value)  => OnAmbientVolumeChanged?.Invoke(value);

        public Action<bool>? OnMuteStateChanged { get; set; }
        
        public Action? OnBackButtonPressed { get; set; }


        public void SetMasterVolume(float volume)
        {
            MasterSliderComponents?.SetValue(volume);
        }

        public void SetMusicVolume(float volume)
        {
            MusicSliderComponents?.SetValue(volume);
        }

        public void SetSFXVolume(float volume)
        {
            SFXSliderComponents?.SetValue(volume);
        }

        public void SetVoiceVolume(float volume)
        {
            VoiceSliderComponents?.SetValue(volume);
        }

        public void SetAmbientVolume(float volume)
        {
            AmbientSliderComponents?.SetValue(volume);
        }
        
        public void SetMuteState(bool mute)
        {
            MuteCheckBox?.SetPressed(mute);
        }
        
        private void HandleMuteToggled(bool e)
        {
            OnMuteStateChanged?.Invoke(e);
        }
        
        private void HandleBackButtonPressed()
        {
            OnBackButtonPressed?.Invoke();
        }

        public override void _Ready()
        {
            base._Ready();
            if (MasterSliderComponents != null) MasterSliderComponents.ValueChanged += EmitOnMasterVolumeChanged;
            if (MusicSliderComponents != null) MusicSliderComponents.ValueChanged += EmitOnMusicVolumeChanged;
            if (SFXSliderComponents != null) SFXSliderComponents.ValueChanged += EmitOnSFXVolumeChanged;
            if (VoiceSliderComponents != null) VoiceSliderComponents.ValueChanged += EmitOnVoiceVolumeChanged;
            if (AmbientSliderComponents != null) AmbientSliderComponents.ValueChanged += EmitOnAmbientVolumeChanged;
            if (MuteCheckBox != null) MuteCheckBox.Toggled += HandleMuteToggled;
             if (BackButton != null) BackButton.Pressed += HandleBackButtonPressed;
            NodeScope = NodeContexts.Instance.RegisterNode<IVolumeSettingsScreen, NodeVolumeSettingsScreenRepo>(this);

        }

        public override void _ExitTree()
        {
            base._ExitTree();
            if (MasterSliderComponents != null) MasterSliderComponents.ValueChanged -= EmitOnMasterVolumeChanged;
            if (MusicSliderComponents != null) MusicSliderComponents.ValueChanged -= EmitOnMusicVolumeChanged;
            if (SFXSliderComponents != null) SFXSliderComponents.ValueChanged -= EmitOnSFXVolumeChanged;
            if (VoiceSliderComponents != null) VoiceSliderComponents.ValueChanged -= EmitOnVoiceVolumeChanged;
            if (AmbientSliderComponents != null) AmbientSliderComponents.ValueChanged -= EmitOnAmbientVolumeChanged;
            if (MuteCheckBox != null) MuteCheckBox.Toggled -= HandleMuteToggled;
            if (BackButton != null) BackButton.Pressed -= HandleBackButtonPressed;

        }

    }
}
