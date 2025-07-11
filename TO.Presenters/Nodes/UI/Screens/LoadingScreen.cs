using Godot;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Abstractions.Nodes.UI.Screens;
using TO.Presenters.Bases;

namespace TO.Presenters.Nodes.UI.Screens;

public class LoadingScreen : NodeRepo<ILoadingScreen>, INodeLoadingScreenRepo
{
    public ProgressBar ProgressBar { get; set; }
    
    public LoadingScreen(ILoadingScreen loadingScreen)
    {
        Node = loadingScreen;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }
    protected override void Init()
    {
        if (Node != null) ProgressBar = Node.ProgressBar;
     
    }
}