using TO.Domains.Models.Repositories.Abstractions.Core.AudioSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.SceneSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.SerializationSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.GodotNodes.Abstractions;
using TO.Nodes.Abstractions.Nodes.Singletons;

namespace TO.Domains.Models.Repositories.Abstractions.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 16:57:50
public class NodeRegister(
   IUIManagerRepo  uiManagerRepo, 
   IAudioManagerRepo audioManagerRepo, 
   ISaveManagerRepo saveManagerRepo,
   ISceneManagerRepo sceneManagerRepo)
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