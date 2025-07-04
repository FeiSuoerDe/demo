using TO.Domains.Models.Repositories.Abstractions.Core.AudioSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.SceneSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.SerializationSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.UISystem;
using TO.Domains.Models.Repositories.Abstractions.Test.Examples;
using TO.GodotNodes.Abstractions;
using TO.Nodes.Abstractions.Nodes.Singletons;
using TO.Nodes.Abstractions.Tests.Examples;

namespace TO.Domains.Models.Repositories.Abstractions.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 16:57:50
public class NodeRegister(
    ITestManagerRepo testManagerRepo,
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
            ITestManager testManager => testManagerRepo.Register(testManager),
            IUIManager uiManager  => uiManagerRepo.Register(uiManager),
            IAudioManager audioManager => audioManagerRepo.Register(audioManager),
            ISaveManager saveManager => saveManagerRepo.Register(saveManager),
            ISceneManager sceneManager => sceneManagerRepo.Register(sceneManager),
            _ => throw new ArgumentException($"暂不支持的单例节点：{typeof(T).Name}")
        };
    }

   
}