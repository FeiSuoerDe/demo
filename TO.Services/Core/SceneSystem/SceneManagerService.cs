using Godot;
using GodotTask;
using TO.Commons.Enums.Infrastructure;
using TO.Commons.Enums.UI;
using TO.Events.Core;
using TO.Repositories.Abstractions.Core.EventBus;
using TO.Repositories.Abstractions.Core.ResourceSystem;
using TO.Repositories.Abstractions.Core.SceneSystem;
using TO.Services.Abstractions.Core.SceneSystem;
using TO.Services.Bases;

namespace TO.Services.Core.SceneSystem;

public class SceneManagerService(ISceneManagerRepo sceneManagerRepo, 
    IResourceLoaderRepo  resourceLoaderRepo,
    IEventBusRepo iEventBusRepo) : BaseService, ISceneManagerService
{
    public async GDTask ChangeScene(string scenePath, TransitionEffectType effectType, float transitionTime = 0.5f)
    {
        // 执行淡出效果
        await sceneManagerRepo.ExecuteTransitionEffect(effectType, transitionTime, false);
        
        // 加载新场景
        var resource = await resourceLoaderRepo.LoadResourceAsync(scenePath, CachePolicy.Temporary,
            ProcessEvent, 1) as PackedScene;
        var newScene = resource?.Instantiate();
        sceneManagerRepo.SceneTree?.Root.AddChild(newScene);
        
        // 移除旧场景
        sceneManagerRepo.CurrentScene?.QueueFree();

        // 更新当前场景
        sceneManagerRepo.CurrentScene = newScene;
        if (sceneManagerRepo.SceneTree != null) sceneManagerRepo.SceneTree.CurrentScene = sceneManagerRepo.CurrentScene;

        // 执行淡入效果
        await sceneManagerRepo.ExecuteTransitionEffect(effectType, transitionTime, true);
    }

    private void ProcessEvent(float progress)
    {
        iEventBusRepo.Publish(new LoadingProgress(progress));
    }
    

}