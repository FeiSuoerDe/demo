using Autofac.Features.Indexed;
using Godot;
using GodotTask;
using TO.Commons.Enums;
using TO.Domains.Eevents.Core;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Core.ResourceSystem;
using TO.Domains.Models.Repositories.Abstractions.Core.SceneSystem;
using TO.Domains.Services.Abstractions.Bases;
using TO.Domains.Services.Abstractions.Core.SceneSystem;

namespace Domains.Core.SceneSystem;

public class SceneManagerService(ISceneManagerRepo sceneManagerRepo, 
    IResourceLoaderRepo  resourceLoaderRepo,
    IIndex<EventEnums,IEventBus> eventBus) : BasesService, ISceneManagerService
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
        eventBus[EventEnums.Domains].Publish(new LoadingProgress(progress));
    }
    

}