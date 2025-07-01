using GodotTask;

namespace TO.Domains.Services.Abstractions.Core.SceneSystem;

public interface ISceneManagerService
{
    GDTask ChangeScene(string scenePath, float transitionTime = 0.5f);
}