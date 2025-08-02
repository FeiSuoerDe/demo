using Godot;
using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.HUD;

public interface ITestBarHud : INode
{
    Slider HSlider { get; set; }
    Label HLabel { get; set; }

    event Action<Guid>? BindModel;

    void Bind(Guid modelId);
}