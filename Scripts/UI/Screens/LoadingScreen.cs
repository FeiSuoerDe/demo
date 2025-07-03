using Contexts;
using demo.UI.Bases;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class LoadingScreen : UIScreen, ILoadingScreen
{
    [Export]
    public ProgressBar ProgressBar { get; set; }

    public override void _Ready()
    {
        NodeScope = NodeContexts.Instance.RegisterNode<ILoadingScreen, NodeLoadingScreenRepo>(this);
        
    }
}