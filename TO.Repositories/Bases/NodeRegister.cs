using TO.Nodes.Abstractions.Bases;
using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Core.AudioSystem;
using TO.Repositories.Abstractions.Core.SceneSystem;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Repositories.Abstractions.Core.UISystem;

namespace TO.Repositories.Bases;

public class NodeRegister(
    IUIManagerRepo  uiManagerRepo,
    IAudioManagerRepo audioManagerRepo,
    ISaveManagerRepo saveManagerRepo,
    ISceneManagerRepo sceneManagerRepo
)
{
    public bool Register<T>(T node) where T : INode
    {
        return node switch
        {
            // 单例
            IUIManager uiManager  => uiManagerRepo.Register(uiManager),
            IAudioManager audioManager => audioManagerRepo.Register(audioManager),
            ISaveManager saveManager => saveManagerRepo.Register(saveManager),
            ISceneManager sceneManager => sceneManagerRepo.Register(sceneManager),
            _ => throw new ArgumentException($"暂不支持的单例节点：{typeof(T).Name}")
        };
    }

   
}