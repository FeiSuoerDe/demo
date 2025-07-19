using Godot;
using System;
using demo.UI.Bases;
using TO.Services.UI.HUD;

public partial class TestBarHud : UIScreen,ITestBarHud
{
    [Export] public Slider HSlider { get; set; }
    [Export] public Label HLabel { get; set; }

    public event Action<Guid>? BindModel;
    
    public override void _Ready()
    {
        base._Ready();
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<ITestBarHud, NodeTestBarHudService>(this);
    }
    public void Bind(Guid modelId)
    {
        BindModel?.Invoke(modelId);
    }
}
