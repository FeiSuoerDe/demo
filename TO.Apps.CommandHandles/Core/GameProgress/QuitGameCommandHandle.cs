using Apps.Commands.Core;
using Godot;
using MediatR;
using TO.Apps.Services.Abstractions.Core.SerializationSystem;

namespace Apps.Core.GameProgress;

public class QuitGameCommandHandle (ISaveManagerAppService saveManagerAppCommand) : IRequestHandler<QuitGameCommand>
{
    public async Task Handle(QuitGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await saveManagerAppCommand.SaveAutosaveAsync();
            var sceneTree = Engine.GetMainLoop() as SceneTree;
            sceneTree?.Quit();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}