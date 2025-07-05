using Godot;
using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace inFras.Nodes.UI.Screens;

public class NodeLoadingScreenRepo : NodeRepo<ILoadingScreen>, INodeLoadingScreenRepo
{
    public ProgressBar ProgressBar { get; set; }
    
    public NodeLoadingScreenRepo(ILoadingScreen loadingScreen)
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