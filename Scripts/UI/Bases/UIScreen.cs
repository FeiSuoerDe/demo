using System;
using Autofac;
using Godot;
using R3;
using TO.Commons.Enums.UI;
using TO.Nodes.Abstractions.UI.Bases;

namespace demo.UI.Bases;


/// <summary>
/// UI屏幕基类，实现了IUIScreen接口
/// </summary>
public abstract partial class UIScreen : Control, IUIScreen
{
    public ILifetimeScope? NodeScope { get; set; }
    
    [Export]
    public bool IsTransparent { get; set; }
    
    [Export]
    public UILayerType LayerType { get; set; } = UILayerType.Normal;
    
    public event Action<bool>? OnVisibilityChanged;

    private CancellationDisposable? Disposable { get; set; }
    
    public bool IsActive => Visible;
    
    public new int ZIndex
    {
        get => base.ZIndex;
        set => base.ZIndex = value;
    }

    public override void _Ready()
    {
        base._Ready();
        Disposable = new CancellationDisposable();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        Disposable?.Dispose();
        NodeScope?.Dispose();
    }
   
    public new virtual void Show()
    {
        Visible = true;
        OnVisibilityChanged?.Invoke(true);
    }
    
    public new virtual void Hide()
    {
        
        Visible = false;
        OnVisibilityChanged?.Invoke(false);
    }
    
    public virtual void Toggle()
    {
        if (Visible)
            Hide();
        else
            Show();
    }
}