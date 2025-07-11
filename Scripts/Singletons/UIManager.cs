using Autofac;
using Godot;
using TO.Nodes.Abstractions.Singletons;
using TO.Nodes.Abstractions.UI.Bases;
using UILayer = demo.UI.Bases.UILayer;

namespace demo.Singletons;

/// <summary>
/// UI管理器，负责管理游戏中的UI屏幕
/// </summary>
public partial class UIManager : Control, IUIManager
{
	
	public ILifetimeScope? NodeScope { get; set; }
	
	public override void _Ready()
	{
		Contexts.Contexts.Instance.RegisterSingleNode<IUIManager>(this);
	}
	
	public IUILayer InitializeUILayer(string name)
	{
		var layer = new UILayer();
		AddChild(layer);
		layer.SetLayerName(name);
		return layer;
	}
}