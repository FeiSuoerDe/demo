#nullable enable

using System.Runtime.CompilerServices;

namespace demo.addons.R3.Godot;

public partial class FrameProviderDispatcher : global::Godot.Node
{
    private StrongBox<double> _processDelta = new StrongBox<double>();
    private StrongBox<double> _physicsProcessDelta = new StrongBox<double>();

    public override void _Ready()
    {
        GodotProviderInitializer.SetDefaultObservableSystem();

        ((GodotFrameProvider)GodotFrameProvider.Process).Delta = _processDelta;
        ((GodotFrameProvider)GodotFrameProvider.PhysicsProcess).Delta = _physicsProcessDelta;
    }

    public override void _Process(double delta)
    {
        _processDelta.Value = delta;
        ((GodotTimeProvider)GodotTimeProvider.Process).Time += delta;
        ((GodotFrameProvider)GodotFrameProvider.Process).Run(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _physicsProcessDelta.Value = delta;
        ((GodotTimeProvider)GodotTimeProvider.PhysicsProcess).Time += delta;
        ((GodotFrameProvider)GodotFrameProvider.PhysicsProcess).Run(delta);
    }
}
