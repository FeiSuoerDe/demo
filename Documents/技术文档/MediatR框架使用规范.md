## 1. 架构定位


1. MediatR仅在应用层(Apps)使用
2. 目的：实现CQRS(命令查询职责分离)模式
3. 不暴露给领域层或基础设施层

## 2. 命令定义规范


1. 使用C# 9+的record类型定义命令
2. 实现MediatR的IRequest或IRequest接口
接口
3. 放在TO.Apps.Commands命名空间下
4. 示例：


```csharp
// 无返回值命令
public record QuitGameCommand : IRequest;

// 有返回值命令
public record GetPlayerDataCommand : IRequest<PlayerData>;


```

## 3. 命令处理器实现规范


1. 实现IRequestHandler或IRequestHandler接口
或IRequestHandler<TCommand, TResponse>接口
2. 放在TO.Apps.CommandHandles命名空间下
3. 通过构造函数注入所需服务
4. 示例：


```csharp
public class QuitGameCommandHandle : IRequestHandler<QuitGameCommand>
{
    private readonly ISaveManagerAppService _saveManager;
    
    public QuitGameCommandHandle(ISaveManagerAppService saveManager)
    {
        _saveManager = saveManager;
    }
    
    public async Task Handle(QuitGameCommand request, CancellationToken cancellationToken)
    {
        await _saveManager.SaveAutosaveAsync();
        SceneTree.Quit();
    }
}


```

## 4. 服务层集成方式


1. 服务应提供清晰的接口(如ISaveManagerAppService)
2. 通过依赖注入IMediator发布命令
3. 复杂操作应封装在服务中，命令处理器只负责协调



### 调用命令


```csharp
// 在任何地方通过IMediator发送命令
await _mediator.Send(new StartGameCommand());


```

## 最佳实践


1. 保持命令简单，只包含必要数据
2. 处理器应专注于协调，业务逻辑放在服务中
3. 使用CancellationToken支持取消操作
4. 合理使用异步(async/await)
5. 复杂流程考虑使用领域事件
