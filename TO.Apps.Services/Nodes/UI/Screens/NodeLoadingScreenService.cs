using Autofac.Features.Indexed;
using Godot;
using GodotTask;
using inFras.Nodes.UI.Screens;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Enums;
using TO.Domains.Eevents;
using TO.Domains.Eevents.Core;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeLoadingScreenService : BaseService
{
    private readonly INodeLoadingScreenRepo _nodeLoadingScreenRepo;
    private readonly IIndex<EventEnums,IEventBus> _eventBus;
    
    public NodeLoadingScreenService(INodeLoadingScreenRepo nodeLoadingScreenRepo, IIndex<EventEnums, IEventBus> eventBus)
    {
        _nodeLoadingScreenRepo = nodeLoadingScreenRepo;
        _eventBus = eventBus;
        _eventBus[EventEnums.Domains].Subscribe<LoadingProgress>(OnLoadingProgress).AddTo(CancellationTokenSource.Token);
    }

    private void OnLoadingProgress(LoadingProgress loadingProgress)
    {
        _nodeLoadingScreenRepo.ProgressBar.Value = loadingProgress.Progress;
    }
}