#nullable enable

using System;
using Godot;
using R3;

namespace demo.addons.R3.Godot;

public static class GodotProviderInitializer
{
    public static void SetDefaultObservableSystem()
    {
        SetDefaultObservableSystem(ex => GD.PrintErr(ex));
    }

    public static void SetDefaultObservableSystem(Action<Exception> unhandledExceptionHandler)
    {
        ObservableSystem.RegisterUnhandledExceptionHandler(unhandledExceptionHandler);
        ObservableSystem.DefaultTimeProvider = GodotTimeProvider.Process;
        ObservableSystem.DefaultFrameProvider = GodotFrameProvider.Process;
    }
}
