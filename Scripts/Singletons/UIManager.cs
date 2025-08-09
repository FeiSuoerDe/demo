using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;
using TO.Nodes.Abstractions.UI.Bases;
using UILayer = TimelapseInvoices.Scripts.UI.Bases.UILayer;

namespace TimelapseInvoices.Scripts.Singletons;

/// <summary>
/// UI管理器，负责管理游戏中的UI屏幕
/// </summary>
public partial class UIManager : Control, IUIManager
{
	
	public ILifetimeScope? NodeScope { get; set; }
	
	public override void _Ready()
	{
		TO.Contexts.Contexts.Instance.RegisterSingleNode<IUIManager>(this);
	}
	
	public IUILayer InitializeUILayer(string name)
	{
		var layer = new UILayer();
		AddChild(layer);
		layer.SetLayerName(name);
		return layer;
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
        
		// 释放依赖注入容器
		NodeScope?.Dispose();
	}
}