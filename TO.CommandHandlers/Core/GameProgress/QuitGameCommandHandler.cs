using Godot;
using MediatR;
using TO.Commands.Core;
using TO.Services.Abstractions.Core.SerializationSystem;

namespace Apps.Core.GameProgress;

public class QuitGameCommandHandler (ISaveManagerService saveManagerService, IMediator mediator) 
    : IRequestHandler<QuitGameCommand>
{
    public async Task Handle(QuitGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            GD.Print("Quitting game....");
            // await saveManagerService.SaveUserSettingsAsync();
            await mediator.Send(new SaveUserSettingsCommand(), cancellationToken);
            var sceneTree = Engine.GetMainLoop() as SceneTree;
            sceneTree?.Quit();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}