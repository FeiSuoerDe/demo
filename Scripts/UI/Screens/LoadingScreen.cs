using Godot;
using TimelapseInvoices.Scripts.UI.Bases;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Services.UI.Screens;

namespace TimelapseInvoices.Scripts.UI.Screens;

public partial class LoadingScreen : UIScreen, ILoadingScreen
{
    [Export]
    public ProgressBar ProgressBar { get; set; }

    
    public override void _Ready()
    {
        // NodeScope = NodeContexts.Instance.RegisterNode<ILoadingScreen, NodeLoadingScreenRepo>(this);
        NodeScope = TO.Contexts.Contexts.Instance.RegisterNode<ILoadingScreen, NodeLoadingScreenService>(this);
    }
    
    public void SetProgressBar(double progress)
    {
        ProgressBar.SetValue(progress);
    }
}