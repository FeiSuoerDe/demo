using TO.Commons.Configs;
using TO.Commons.Enums.System;
using TO.Nodes.Abstractions.UI.Trigger;
using TO.Services.Abstractions.Core.AudioSystem;
using TO.Services.Bases;

namespace TO.Services.UI.Trigger;

public class NodeObservableSoundReactorService : BaseService
{
    private readonly IObservableSoundReactor _observableSoundReactor;
    private readonly IAudioManagerService  _audioManagerService;

    private string  _path;
    
    public NodeObservableSoundReactorService(IObservableSoundReactor observableSoundReactor, IAudioManagerService audioManagerService)
    {
        _observableSoundReactor = observableSoundReactor;
        _audioManagerService = audioManagerService;
        if (_observableSoundReactor.Trigger != null) _observableSoundReactor.Trigger.Triggered += Triggered;
        _path = AudioConfigs.AudioPath[_observableSoundReactor.Audio];
    }

    private void Triggered(Dictionary<string, object>? obj)
    {
        switch (_observableSoundReactor.AudioType)
        {
            case AudioEnums.AudioType.Music:
                _audioManagerService.PlayMusic(_path, _observableSoundReactor.FadeDuration, _observableSoundReactor.Loop);
                break;
            case AudioEnums.AudioType.SoundEffect:
                _audioManagerService.PlaySound(_path, _observableSoundReactor.Volume);
                break;
            case AudioEnums.AudioType.Voice:
                _audioManagerService.PlayVoice(_path, _observableSoundReactor.Volume);
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
        if (_observableSoundReactor.Trigger != null) _observableSoundReactor.Trigger.Triggered -= Triggered;
    }
  
}