using Godot;
using GodotTask;
using TO.Domains.Models.Repositories.Abstractions.Bases;
using TO.Nodes.Abstractions.Nodes.Singletons;

namespace TO.Domains.Models.Repositories.Abstractions.Core.SceneSystem;

public interface ISceneManagerRepo : ISingletonNodeRepo<ISceneManager>
{
    CanvasLayer? CanvasLayer { get; set; }
    ColorRect? ColorRect { get; set; }
    Node? CurrentScene { get; set; }
    SceneTree? SceneTree { get; set; }

    GDTask FadeOut(float time);

    GDTask FadeIn(float time);
}