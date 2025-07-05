using MediatR;

namespace Apps.Commands.Core;

public record StartGameCommand : IRequest;

public record QuitGameCommand : IRequest;