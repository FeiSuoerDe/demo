using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.UI.Screens;

public interface ILoadingScreen : INode
{
    void SetProgressBar(double progress);
}