using GodotTask;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Bases;

namespace TO.Services.UI.Screens;

public class NodeLoadingScreenService : BaseService
{
    private readonly ILoadingScreen _nodeLoadingScreen;
    private readonly IEventBusRepo _iEventBusRepo;
    
    public NodeLoadingScreenService(ILoadingScreen nodeLoadingScreen, IEventBusRepo iEventBusRepo)
    {
        _nodeLoadingScreen = nodeLoadingScreen;
        _iEventBusRepo = iEventBusRepo;
        _iEventBusRepo.Subscribe<LoadingProgress>(OnLoadingProgress).AddTo(CancellationTokenSource.Token);
    }

    private void OnLoadingProgress(LoadingProgress loadingProgress)
    {
        _nodeLoadingScreen.SetProgressBar(loadingProgress.Progress);
    }
}