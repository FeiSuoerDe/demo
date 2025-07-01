using System;
using Godot;

namespace demo.UI.Components;

public partial class SliderComponents : Node
{
    [Export] public Slider? Slider { get; set; }
    [Export] public Label? Label;

    public Action<double>? ValueChanged { get; set; }
    private void EmitValueChanged(double value)  => ValueChanged?.Invoke(value);

    public override void _Ready()
    {
        if (Slider != null) Slider.ValueChanged += EmitValueChanged;
    }

    public void SetValue(float value)
    {
        Slider?.SetValue(value);
        if (Label != null) Label.Text = $"Volume: {value:P0}";
    }
    
    public override void _ExitTree(){
    {
        if (Slider != null) Slider.ValueChanged -= EmitValueChanged;
    }}
}