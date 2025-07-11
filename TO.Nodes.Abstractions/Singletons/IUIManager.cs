using TO.Nodes.Abstractions.Bases;
using TO.Nodes.Abstractions.UI.Bases;

namespace TO.Nodes.Abstractions.Singletons;

/// <summary>
/// UI管理器接口，负责管理游戏中的UI屏幕
/// </summary>
public interface IUIManager : INode
{
    IUILayer InitializeUILayer(string name);
}