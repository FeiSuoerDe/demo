
#nullable enable

using System;
using Godot;
using R3;
using GDArray = Godot.Collections.Array;

namespace demo.addons.R3.Godot;

// Sends runtime ObservableTracker information to ObservableTrackerDebuggerPlugin.
// Needs to be an Autoload. Should not be instantiated manually.
public partial class ObservableTrackerRuntimeHook : Node
{
    public override void _Ready()
    {
#if TOOLS
        EngineDebugger.RegisterMessageCapture(ObservableTrackerDebuggerPlugin.MessageHeader, Callable.From((string message, GDArray data) =>
        { 
            //GD.Print(nameof(ObservableTrackerRuntimeHook) + " received " + message);
            switch (message)
            {
                case ObservableTrackerDebuggerPlugin.MessageRequestActiveTasks:
                    // data[0]: If true, force an update anyway.
                    if (ObservableTracker.CheckAndResetDirty() || data[0].AsBool())
                    {
                        GDArray states = new();
                        ObservableTracker.ForEachActiveTask(state =>
                        {
                            // DateTime is not a Variant type, so we serialize it using Ticks instead.
                            states.Add(new GDArray { state.TrackingId, state.FormattedType, state.AddTime.Ticks, state.StackTrace });
                        });
                        EngineDebugger.SendMessage(ObservableTrackerDebuggerPlugin.MessageHeader + ":" + ObservableTrackerDebuggerPlugin.MessageReceiveActiveTasks, new () { true, states });
                    }
                    else
                    {
                        EngineDebugger.SendMessage(ObservableTrackerDebuggerPlugin.MessageHeader + ":" + ObservableTrackerDebuggerPlugin.MessageReceiveActiveTasks, new () { false, });
                    }
                    break;
                case ObservableTrackerDebuggerPlugin.MessageSetEnableStates:
                    ObservableTracker.EnableTracking = data[0].AsBool();
                    ObservableTracker.EnableStackTrace = data[1].AsBool();
                    break;
                case ObservableTrackerDebuggerPlugin.MessageInvokeGcCollect:
                    GC.Collect(0);
                    break;
            }
            return true;
        }));
#endif
    }

    public override void _ExitTree()
    {
#if TOOLS
        EngineDebugger.UnregisterMessageCapture(ObservableTrackerDebuggerPlugin.MessageHeader);
#endif
    }
}
