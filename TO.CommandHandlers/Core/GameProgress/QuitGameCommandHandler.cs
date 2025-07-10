using Apps.Commands.Core;
using Godot;
using MediatR;
using TO.Services.Abstractions.Core.SerializationSystem;

namespace Apps.Core.GameProgress;

public class QuitGameCommandHandler (ISaveManagerAppService saveManagerAppCommand) 
    : IRequestHandler<QuitGameCommand>
{
    public async Task Handle(QuitGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            GD.Print("Quitting game....");
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