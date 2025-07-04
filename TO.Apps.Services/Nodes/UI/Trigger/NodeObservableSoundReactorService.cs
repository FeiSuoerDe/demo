using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Configs;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Trigger;
using TO.Domains.Services.Abstractions.Core.AudioSystem;

namespace TO.Apps.Services.Node.UI.Trigger;

public class NodeObservableSoundReactorService : BaseService
{
    private readonly IObservableSoundReactorRepo _observableSoundReactorRepo;
    private readonly IAudioManagerService  _audioManagerService;

    private string  _path;
    
    public NodeObservableSoundReactorService(IObservableSoundReactorRepo observableSoundReactorRepo, IAudioManagerService audioManagerService)
    {
        _observableSoundReactorRepo = observableSoundReactorRepo;
        _audioManagerService = audioManagerService;
        if (_observableSoundReactorRepo.Trigger != null) _observableSoundReactorRepo.Trigger.Triggered += Triggered;
        _path = AudioConfigs.AudioPath[_observableSoundReactorRepo.Audio];
    }

    private void Triggered(Dictionary<string, object>? obj)
    {
        switch (_observableSoundReactorRepo.AudioType)
        {
            case AudioEnums.AudioType.Music:
                _audioManagerService.PlayMusic(_path, _observableSoundReactorRepo.FadeDuration, _observableSoundReactorRepo.Loop);
                break;
            case AudioEnums.AudioType.SoundEffect:
                _audioManagerService.PlaySound(_path, _observableSoundReactorRepo.Volume);
                break;
            case AudioEnums.AudioType.Voice:
                _audioManagerService.PlayVoice(_path, _observableSoundReactorRepo.Volume);
                break;
            case AudioEnums.AudioType.Ambient:
                break;
            case AudioEnums.AudioType.Master:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected  override void UnSubscriber()
    {
        base.UnSubscriber();
        if (_observableSoundReactorRepo.Trigger != null) _observableSoundReactorRepo.Trigger.Triggered -= Triggered;
    }
  
}