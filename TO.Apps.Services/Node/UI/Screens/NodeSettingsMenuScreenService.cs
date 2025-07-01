using Autofac.Features.Indexed;
using Godot;
using TO.Apps.Events;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeSettingsMenuScreenService: BaseService
{
    private readonly ISettingsMenuScreenRepo _settingsMenuScreenRepo;
    private readonly IEventBus  _eventBus;
    public NodeSettingsMenuScreenService(ISettingsMenuScreenRepo settingsMenuScreenRepo,
        IIndex<EventEnums,IEventBus> eventBus)
    {
        _settingsMenuScreenRepo = settingsMenuScreenRepo;
        _eventBus = eventBus[EventEnums.UI];
        _settingsMenuScreenRepo.AudioButtonPressed += OnAudioButtonPressed;
        _settingsMenuScreenRepo.VideoButtonPressed += OnVideoButtonPressed;
        _settingsMenuScreenRepo.BackButtonPressed += OnBackButtonPressed;
        
    }

    private void OnAudioButtonPressed()
    {
        _eventBus.Publish(new ShowUI(UIName.VolumeSettingsScreen));
       
    }

    private void OnVideoButtonPressed()
    {
    }

    private void OnBackButtonPressed()
    {
        _eventBus.Publish(new Hide());
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _settingsMenuScreenRepo.AudioButtonPressed -= OnAudioButtonPressed;
        _settingsMenuScreenRepo.VideoButtonPressed -= OnVideoButtonPressed;
        _settingsMenuScreenRepo.BackButtonPressed -= OnBackButtonPressed;
    }

   
}