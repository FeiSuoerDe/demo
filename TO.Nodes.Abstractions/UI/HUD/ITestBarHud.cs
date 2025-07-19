using Godot;

using TO.Nodes.Abstractions.Bases;

public interface ITestBarHud : INode
{
    Slider HSlider { get; set; }
    Label HLabel { get; set; }

    event Action<Guid>? BindModel;

    void Bind(Guid modelId);
}
