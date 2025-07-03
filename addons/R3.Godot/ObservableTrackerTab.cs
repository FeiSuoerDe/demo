#if TOOLS
#nullable enable

using Godot;

namespace demo.addons.R3.Godot;

[Tool]
public partial class ObservableTrackerTab : VBoxContainer
{
    public const string EnableAutoReloadKey = "ObservableTracker_EnableAutoReloadKey";
    public const string EnableTrackingKey = "ObservableTracker_EnableTrackingKey";
    public const string EnableStackTraceKey = "ObservableTracker_EnableStackTraceKey";
    private bool _enableAutoReload, _enableTracking, _enableStackTrace;
    private ObservableTrackerTree? _tree;
    private ObservableTrackerDebuggerPlugin? _debuggerPlugin;
    private int _interval = 0;
    private int _sessionId = 0;

    public void NotifyOnSessionSetup(ObservableTrackerDebuggerPlugin debuggerPlugin, int sessionId)
    {
        
        this._debuggerPlugin = debuggerPlugin;
        this._sessionId = sessionId;
        _tree ??= new ObservableTrackerTree();
        _tree.NotifyOnSessionSetup(debuggerPlugin, sessionId);
    }

    public void NotifyOnSessionStart()
    {
        _debuggerPlugin?.SetEnableStates(_sessionId, _enableTracking, _enableStackTrace);
    }

    public override void _Ready()
    {
        Name = "Observable Tracker";

        _tree ??= new ObservableTrackerTree();
        
        // Head panel
        var headPanelLayout = new HBoxContainer();
        headPanelLayout.SetAnchor(Side.Left, 0);
        headPanelLayout.SetAnchor(Side.Right, 0);
        AddChild(headPanelLayout);

        // Toggle buttons (top left)
        var enableAutoReloadToggle = new CheckButton
        {
            Text = "Enable AutoReload",
            TooltipText = "Reload automatically."
        };
        var enableTrackingToggle = new CheckButton
        {
            Text = "Enable Tracking",
            TooltipText = "Start to track Observable subscription. Performance impact: low"
        };
        var enableStackTraceToggle = new CheckButton
        {
            Text = "Enable StackTrace",
            TooltipText = "Capture StackTrace when subscribed. Performance impact: high"
        };

        // For every button: Initialize pressed state and subscribe to Toggled event.
        EditorSettings settings = EditorInterface.Singleton.GetEditorSettings();
        enableAutoReloadToggle.ButtonPressed = _enableAutoReload = GetSettingOrDefault(settings, EnableAutoReloadKey, false).AsBool();
        enableAutoReloadToggle.Toggled += toggledOn =>
        {
            settings.SetSetting(EnableAutoReloadKey, toggledOn);
            _enableAutoReload = toggledOn;
        };
        enableTrackingToggle.ButtonPressed = _enableTracking = GetSettingOrDefault(settings, EnableTrackingKey, false).AsBool();
        enableTrackingToggle.Toggled += toggledOn =>
        {
            
            settings.SetSetting(EnableTrackingKey, toggledOn);
            _enableTracking = toggledOn;
            _debuggerPlugin?.SetEnableStates(_sessionId, _enableTracking, _enableStackTrace);
        };
        enableStackTraceToggle.ButtonPressed = _enableStackTrace = GetSettingOrDefault(settings, EnableStackTraceKey, false).AsBool();
        enableStackTraceToggle.Toggled += toggledOn =>
        {
            
            settings.SetSetting(EnableStackTraceKey, toggledOn);
            _enableStackTrace = toggledOn;
            _debuggerPlugin?.SetEnableStates(_sessionId, _enableTracking, _enableStackTrace);
        };

        // Regular buttons (top right)
        var reloadButton = new Button
        {
            Text = "Reload",
            TooltipText = "Reload View."
        };
        var gcButton = new Button
        {
            Text = "GC.Collect",
            TooltipText = "Invoke GC.Collect."
        };

        reloadButton.Pressed += () =>
        {
           
            _debuggerPlugin?.UpdateTrackingStates(_sessionId, true);
        };
        gcButton.Pressed += () =>
        {
            
            _debuggerPlugin?.InvokeGcCollect(_sessionId);
        };

        // Button layout.
        headPanelLayout.AddChild(enableAutoReloadToggle);
        headPanelLayout.AddChild(enableTrackingToggle);
        headPanelLayout.AddChild(enableStackTraceToggle);
        // Kind of like Unity's FlexibleSpace. Pushes the first three buttons to the left, and the remaining buttons to the right.
        headPanelLayout.AddChild(new Control()
        {
            SizeFlagsHorizontal = SizeFlags.Expand,
        });
        headPanelLayout.AddChild(reloadButton);
        headPanelLayout.AddChild(gcButton);

        // Tree goes last.
        AddChild(_tree);
    }

    public override void _Process(double delta)
    {
        if (!_enableAutoReload) return;
        if (_interval++ % 120 == 0)
        {
                
            _debuggerPlugin?.UpdateTrackingStates(_sessionId);
        }
    }

    private static Variant GetSettingOrDefault(EditorSettings settings, string key, Variant @default)
    {
        if (settings.HasSetting(key))
        {
            return settings.GetSetting(key);
        }
        else
        {
            return @default;
        }
    }
}
#endif
