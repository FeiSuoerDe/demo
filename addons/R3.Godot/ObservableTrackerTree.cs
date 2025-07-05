#if TOOLS
#nullable enable

using System;
using System.Collections.Generic;
using Godot;
using R3;

namespace demo.addons.R3.Godot;

[Tool]
public partial class ObservableTrackerTree : Tree
{
    private ObservableTrackerDebuggerPlugin? _debuggerPlugin;
    private int _sessionId;
    public void NotifyOnSessionSetup(ObservableTrackerDebuggerPlugin debuggerPlugin, int sessionId)
    {
        this._debuggerPlugin = debuggerPlugin;
        this._sessionId = sessionId;
        debuggerPlugin!.RegisterReceivedActiveTasks(sessionId, Reload);
        Clear();
    }

    public override void _Ready()
    {
        AllowReselect = false;
        Columns = 3;
        ColumnTitlesVisible = true;
        SetColumnTitle(0, "Type");
        SetColumnTitle(1, "Elapsed");
        SetColumnTitle(2, "StackTrace");
        SetColumnExpand(0, true);
        SetColumnExpand(1, true);
        SetColumnExpand(2, true);
        SetColumnExpandRatio(0, 3);
        SetColumnExpandRatio(1, 1);
        SetColumnExpandRatio(2, 6);
        SetColumnClipContent(0, true);
        SetColumnClipContent(1, true);
        SetColumnClipContent(2, true);
        HideRoot = true;
        SizeFlagsVertical = SizeFlags.ExpandFill;
    }

    public override void _ExitTree()
    {
        _debuggerPlugin!.UnregisterReceivedActiveTasks(_sessionId, Reload);
    }

    public void Reload(IEnumerable<TrackingState> states)
    {
        Clear();
        TreeItem root = CreateItem();
        foreach(TrackingState state in states)
        {
            TreeItem row = CreateItem(root);
            var now = DateTime.Now;
            // Type
            row.SetText(0, state.FormattedType);
            // Elapsed
            row.SetText(1, (now - state.AddTime).TotalSeconds.ToString("00.00"));
            // StackTrace
            row.SetText(2, state.StackTrace);
        };
    }
}
#endif
