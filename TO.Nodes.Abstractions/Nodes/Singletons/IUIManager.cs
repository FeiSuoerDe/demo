using TO.GodotNodes.Abstractions;
using TO.Nodes.Abstractions.Nodes.UI.Bases;

namespace TO.Nodes.Abstractions.Nodes.Singletons;

/// <summary>
/// UI管理器接口，负责管理游戏中的UI屏幕
/// </summary>
public interface IUIManager : INode
{
    IUILayer InitializeUILayer(string name);
}