using System;
using demo.UI.Bases;
using Godot;
using TO.Nodes.Abstractions.UI.HUD;
using TO.Services.UI.HUD;

namespace demo.UI.HUD;

public partial class TestBarHud : UIScreen,ITestBarHud
{
    [Export] public Slider HSlider { get; set; }
    [Export] public Label HLabel { get; set; }

    [Export] public Label ConLabel { get; set; }

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