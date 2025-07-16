using MediatR;

namespace TO.Commands.Core;

/// <summary>
/// 保存用户设置命令
/// </summary>
public record SaveUserSettingsCommand : IRequest;

/// <summary>
/// 加载用户设置命令
/// </summary>
public record LoadUserSettingsCommand : IRequest;

