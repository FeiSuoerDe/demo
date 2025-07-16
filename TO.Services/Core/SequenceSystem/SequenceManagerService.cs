using MediatR;
using TO.Commands.Core;
using TO.Services.Abstractions.Core.SequenceSystem;

namespace TO.Services.Core.SequenceSystem;

public class SequenceManagerService : ISequenceManagerService
{
    private readonly IMediator _mediator;
    
    public SequenceManagerService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Send(new LoadUserSettingsCommand());
    }
}