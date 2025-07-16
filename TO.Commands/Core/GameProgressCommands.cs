using MediatR;

namespace TO.Commands.Core;

public record StartGameCommand : IRequest;

public record QuitGameCommand : IRequest;