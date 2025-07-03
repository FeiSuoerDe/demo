#if TOOLS
#nullable enable

using Godot;

namespace demo.addons.R3.Godot;

[Tool]
public partial class GodotR3Plugin : EditorPlugin
{
    private static ObservableTrackerDebuggerPlugin? _observableTrackerDebugger;
    public override void _EnterTree()
    {
        _observableTrackerDebugger ??= new ObservableTrackerDebuggerPlugin();
        AddDebuggerPlugin(_observableTrackerDebugger);
        // Automatically install autoloads here for ease of use.
        AddAutoloadSingleton(nameof(FrameProviderDispatcher), "res://addons/R3.Godot/FrameProviderDispatcher.cs");
        AddAutoloadSingleton(nameof(ObservableTrackerRuntimeHook), "res://addons/R3.Godot/ObservableTrackerRuntimeHook.cs");
    }

    public override void _ExitTree()
    {
        if (_observableTrackerDebugger != null)
        {
            RemoveDebuggerPlugin(_observableTrackerDebugger);
            _observableTrackerDebugger = null;
        }
        RemoveAutoloadSingleton(nameof(FrameProviderDispatcher));
        RemoveAutoloadSingleton(nameof(ObservableTrackerRuntimeHook));
    }
}
#endif
