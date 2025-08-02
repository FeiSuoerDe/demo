using TO.Commons.Enums.UI;
using TO.Events.Core;
using TO.Nodes.Abstractions.UI.Screens;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Services.Bases;

namespace TO.Services.UI.Screens;

public class NodeSettingsMenuScreenService: BaseService
{
    private readonly ISettingsMenuScreen _settingsMenuScreen;
    private readonly IEventBusRepo  _iEventBusRepo;
    public NodeSettingsMenuScreenService(ISettingsMenuScreen settingsMenuScreen,
        IEventBusRepo eventBus)
    {
        _settingsMenuScreen = settingsMenuScreen;
        _iEventBusRepo = eventBus;
        _settingsMenuScreen.OnAudioButtonPressed += OnAudioButtonPressed;
        _settingsMenuScreen.OnVideoButtonPressed += OnVideoButtonPressed;
        _settingsMenuScreen.OnBackButtonPressed += OnBackButtonPressed;
        
    }

    private void OnAudioButtonPressed()
    {
        _iEventBusRepo.Publish(new ShowUI(UIName.VolumeSettingsScreen));
       
    }

    private void OnVideoButtonPressed()
    {
    }

    private void OnBackButtonPressed()
    {
        _iEventBusRepo.Publish(new HideUI());
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _settingsMenuScreen.OnAudioButtonPressed -= OnAudioButtonPressed;
        _settingsMenuScreen.OnVideoButtonPressed -= OnVideoButtonPressed;
        _settingsMenuScreen.OnBackButtonPressed -= OnBackButtonPressed;
    }

   
}