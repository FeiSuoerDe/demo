
# 基于Autofac的倒置节点层实现方案

node实例化时，先在NodeContext创建子作用域，在子作用域注册节点的接口和Infars层的repo，并且创建repo实例绑定node的生命周期。repo构造函数在Context创建子作用域，并且注册对应的被依赖的服务和command等。当node被释放时，子作用域也会释放。

## 一、类命名规范

### （一）节点类命名规范

#### 1. **表示层节点（主项目）**
- **UI节点**：直接使用业务名称，如 `MainMenuScreen`、`GameplayScreen`、`SettingsScreen`
- **游戏节点**：直接使用业务名称，如 `Player`、`Enemy`、`Weapon`
- **管理器节点**：使用 `Manager` 后缀，如 `SceneManager`、`AudioManager`

#### 2. **应用层节点（TO.Apps.Services）**
- **所有节点类**：使用 `Node` 前缀 + 业务名称，如：
  - `NodeMainMenuScreen`
  - `NodeGameplayScreen` 
  - `NodeInventoryPanel`
  - `NodePlayerController`

#### 3. **基础设施层节点（TO.Infras.Repositories）**
- **所有节点类**：使用 `Node` 前缀 + 业务名称，如：
  - `NodeDataPersistence`
  - `NodeFileManager`
  - `NodeNetworkClient`
  - `NodeCacheManager`

#### 4. **节点接口命名**
- **表示层接口**：`I` + 节点名称，如 `IMainMenuScreen`、`IPlayer`
- **应用层接口**：`I` + `Node` + 业务名称，如 `INodeMainMenuScreen`
- **基础设施层接口**：`I` + `Node` + 业务名称，如 `INodeDataPersistence`

#### 5. **仓储类命名**
- **所有仓储类**：节点名称 + `Repo` 后缀，如：
  - `MainMenuScreenRepo`（表示层）
  - `NodeMainMenuScreenRepo`（应用层）
  - `NodeDataPersistenceRepo`（基础设施层）

### （二）命名规范示例对照表

| 层级 | 节点类型 | 节点类名 | 接口名 | 仓储类名 |
|------|----------|----------|--------|---------|
| 表示层 | UI屏幕 | `MainMenuScreen` | `IMainMenuScreen` | `MainMenuScreenRepo` |
| 表示层 | 游戏对象 | `Player` | `IPlayer` | `PlayerRepo` |
| 应用层 | UI服务 | `NodeMainMenuScreen` | `INodeMainMenuScreen` | `NodeMainMenuScreenRepo` |
| 应用层 | 游戏服务 | `NodePlayerController` | `INodePlayerController` | `NodePlayerControllerRepo` |
| 基础设施层 | 数据服务 | `NodeDataPersistence` | `INodeDataPersistence` | `NodeDataPersistenceRepo` |
| 基础设施层 | 网络服务 | `NodeNetworkClient` | `INodeNetworkClient` | `NodeNetworkClientRepo` |

### （三）命名规范的优势

1. **层级清晰**：通过 `Node` 前缀明确区分应用层和基础设施层的节点
2. **避免冲突**：防止不同层级间的类名冲突
3. **易于维护**：快速识别节点所属层级，便于代码维护
4. **统一标准**：团队开发时遵循统一的命名约定

## 二、核心设计思路

采用Autofac作为依赖注入容器，实现Godot节点与依赖服务的生命周期绑定。通过为每个节点创建独立的生命周期作用域（Lifetime Scope），实现资源的精确控制和依赖隔离。

## 二、技术实现方案

### （一）架构概览


```
RootContainer (全局注册)
│
├─ NodeLifetimeScope (节点级作用域)
│  ├─ IMainMenuScreen → MainMenuScreen (表示层)
│  ├─ INodeMainMenuScreen → NodeMainMenuScreen (应用层)
│  ├─ INodeDataPersistence → NodeDataPersistence (基础设施层)
│  └─ MainMenuScreenRepo → 实例
│     └─ ILifetimeScope → 节点专属作用域
│
└─ 其他节点作用域...
```

### （二）关键组件实现

#### 1. **依赖注入上下文管理**


```csharp
public class NodeContexts : LazySingleton<NodeContexts>
{
    private IContainer Container { get; }
    
    public NodeContexts()
    {
        var builder = new ContainerBuilder();
        Container = builder.Build();
    }
    
    public ILifetimeScope RegisterNode<TNode, TRepo>(TNode scene)
        where TNode : class, INode
        where TRepo : class, INodeRepo<TNode>
    {
        var scope = Container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(scene).As<INode>().As<TNode>().SingleInstance();
            builder.RegisterType<TRepo>().InstancePerLifetimeScope();
        });

        scope.Resolve<TRepo>();
        return scope;
    }
}


```

#### 2. **节点基类设计**


```csharp
public interface INode
{
    // 节点相关接口定义
}

// 表示层节点基类
public abstract class UiScreen : Control, INode
{
    protected ILifetimeScope? NodeScope { get; set; }

    public virtual void _Ready()
    {
        // 基类实现
    }

    protected virtual void RegisterCallbacks()
    {
        // 注册回调方法
    }

    protected virtual void UnregisterCallbacks()
    {
        // 取消注册回调方法
    }
}

// 应用层节点基类
public abstract class NodeUiScreen : Control, INode
{
    protected ILifetimeScope? NodeScope { get; set; }

    public virtual void _Ready()
    {
        // 应用层节点基类实现
    }

    protected virtual void RegisterCallbacks()
    {
        // 注册回调方法
    }

    protected virtual void UnregisterCallbacks()
    {
        // 取消注册回调方法
    }
}

// 基础设施层节点基类
public abstract class NodeInfrastructure : Node, INode
{
    protected ILifetimeScope? NodeScope { get; set; }

    public virtual void _Ready()
    {
        // 基础设施层节点基类实现
    }

    protected virtual void RegisterCallbacks()
    {
        // 注册回调方法
    }

    protected virtual void UnregisterCallbacks()
    {
        // 取消注册回调方法
    }
}


```

#### 3. **仓储模式实现**


```csharp
public interface INodeRepo<TNode> where TNode : class, INode
{
    event Action? Ready;
    event Action? TreeExiting;
    TNode? Node { get; }
}

public abstract class NodeRepo<TNode> : INodeRepo<TNode> where TNode : class, INode
{
    public event Action? Ready;
    private void EmitReady() => Ready?.Invoke();
    
    public event Action? TreeExiting;
    private void EmitTreeExiting() => TreeExiting?.Invoke();
    
    public TNode? Node { get; set; }
    private ILifetimeScope? RepoScope { get; set; }

    protected virtual void ConfigureNodeScope(ILifetimeScope lifetimeScope)
    {
        RepoScope = lifetimeScope;
    }

    public void Register()
    {
        if (Node != null)
        {
            Node.TreeExiting += Unregister;
            Node.Ready += EmitReady;
            Node.TreeExiting += EmitTreeExiting;
        }

        ConnectNodeEvents();
    }

    private void Unregister()
    {
        if (Node == null)
        {
            GD.PrintErr("很奇怪，单例节点取消注册时已经为空了！");
            return;
        }

        Node.TreeExiting -= Unregister;
        Node.Ready -= EmitReady;
        Node.TreeExiting -= EmitTreeExiting;
        DisconnectNodeEvents();
        RepoScope?.Dispose();
        Node = default;
    }

    protected virtual void ConnectNodeEvents()
    {
    }

    protected virtual void DisconnectNodeEvents()
    {
    }
}


```

#### 4. **具体节点实现**

##### 表示层节点实现
```csharp
// 表示层：主菜单屏幕（主项目中）
// Scripts/UI/Screens/MainMenuScreen.cs
// 文件名: MainMenuScreen.cs
// 功能: 主菜单界面，包含开始游戏、设置和退出游戏按钮

using System;
using Contexts;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class MainMenuScreen : UiScreen, IMainMenuScreen
{
	[Export]
	public Button? StartButton { get; set; }
	
	[Export]
	public Button? SettingsButton { get; set; }
	
	[Export]
	public Button? ExitButton { get; set; }
	
	public Action? OnStartButtonPressed { get; set; }
	private void EmitStartButtonPressed() => OnStartButtonPressed?.Invoke();
	
	public Action? OnSettingsButtonPressed { get; set; }
	private void EmitSettingsButtonPressed() => OnSettingsButtonPressed?.Invoke();
	
	public Action? OnExitButtonPressed { get; set; }
	private void EmitExitButtonPressed() => OnExitButtonPressed?.Invoke();

	public override void _Ready()
	{
		base._Ready();
		NodeScope = NodeContexts.Instance.RegisterNode<IMainMenuScreen, MainMenuScreenRepo>(this);
	}

	protected override void RegisterCallbacks()
	{
		if (StartButton != null) StartButton.Pressed += EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed += EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed += EmitExitButtonPressed;
	}

	protected override void UnregisterCallbacks()
	{
		if (StartButton != null) StartButton.Pressed -= EmitStartButtonPressed;
		if (SettingsButton != null) SettingsButton.Pressed -= EmitSettingsButtonPressed;
		if (ExitButton != null) ExitButton.Pressed -= EmitExitButtonPressed;
	}
}

// Scripts/UI/Screens/LoadingScreen.cs
using Contexts;
using demo.UI.Bases;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class LoadingScreen : UIScreen, ILoadingScreen
{
    [Export]
    public ProgressBar ProgressBar { get; set; }

    public override void _Ready()
    {
        NodeScope = NodeContexts.Instance.RegisterNode<ILoadingScreen, NodeLoadingScreenRepo>(this);
        
    }
}
```

##### 应用层节点服务实现
```csharp
// TO.Apps.Services/Node/UI/Screens/NodeMainMenuScreenService.cs
using MediatR;
using R3;
using TO.Commands.Abstractions.Commands.Game;
using TO.Events.Abstractions.Events.UI;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeMainMenuScreenService : IDisposable
{
    private readonly IMainMenuScreen _screen;
    private readonly IMediator _mediator;
    private readonly CompositeDisposable _disposables = new();
    
    public NodeMainMenuScreenService(IMainMenuScreen screen, IMediator mediator)
    {
        _screen = screen;
        _mediator = mediator;
        Initialize();
    }
    
    private void Initialize()
    {
        // 订阅按钮事件
        _screen.OnStartButtonPressed += HandleStartGame;
        _screen.OnSettingsButtonPressed += HandleOpenSettings;
        _screen.OnExitButtonPressed += HandleExitGame;
    }
    
    private async void HandleStartGame()
    {
        await _mediator.Send(new StartGameCommand());
    }
    
    private async void HandleOpenSettings()
    {
        await _mediator.Publish(new OpenSettingsEvent());
    }
    
    private async void HandleExitGame()
    {
        await _mediator.Send(new ExitGameCommand());
    }
    
    public void Dispose()
    {
        _screen.OnStartButtonPressed -= HandleStartGame;
        _screen.OnSettingsButtonPressed -= HandleOpenSettings;
        _screen.OnExitButtonPressed -= HandleExitGame;
        _disposables.Dispose();
    }
}

// TO.Apps.Services/Node/UI/Screens/NodeLoadingScreenService.cs
using R3;
using TO.Events.Abstractions.Events.Loading;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeLoadingScreenService : IDisposable
{
    private readonly ILoadingScreen _screen;
    private readonly CompositeDisposable _disposables = new();
    
    public NodeLoadingScreenService(ILoadingScreen screen)
    {
        _screen = screen;
        Initialize();
    }
    
    private void Initialize()
    {
        // 订阅加载进度事件
        Observable.FromEvent<LoadingProgressEvent>(
            h => LoadingEvents.ProgressUpdated += h,
            h => LoadingEvents.ProgressUpdated -= h)
            .Subscribe(OnProgressUpdated)
            .AddTo(_disposables);
    }
    
    private void OnProgressUpdated(LoadingProgressEvent progressEvent)
    {
        if (_screen.ProgressBar != null)
        {
            _screen.ProgressBar.Value = progressEvent.Progress;
        }
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}
```

##### 基础设施层节点仓储实现
```csharp
// TO.Infras.Repositories/Nodes/UI/Screens/NodeMainMenuScreenRepo.cs
using Autofac;
using Contexts;
using Godot;
using TO.Infras.Repositories.Bases;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace inFras.Nodes.UI.Screens;

public class NodeMainMenuScreenRepo : NodeRepo<IMainMenuScreen>
{
    public NodeMainMenuScreenRepo(IMainMenuScreen mainMenuScreen)
    {
        ContextEvents.TriggerRegisterNode(nameof(NodeMainMenuScreenRepo), this, ConfigureNodeScope);
        Node = mainMenuScreen;
        Register();
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        if (Node?.OnStartButtonPressed != null)
        {
            Node.OnStartButtonPressed += HandleStartButtonPressed;
        }
        if (Node?.OnSettingsButtonPressed != null)
        {
            Node.OnSettingsButtonPressed += HandleSettingsButtonPressed;
        }
        if (Node?.OnExitButtonPressed != null)
        {
            Node.OnExitButtonPressed += HandleExitButtonPressed;
        }
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        if (Node?.OnStartButtonPressed != null)
        {
            Node.OnStartButtonPressed -= HandleStartButtonPressed;
        }
        if (Node?.OnSettingsButtonPressed != null)
        {
            Node.OnSettingsButtonPressed -= HandleSettingsButtonPressed;
        }
        if (Node?.OnExitButtonPressed != null)
        {
            Node.OnExitButtonPressed -= HandleExitButtonPressed;
        }
    }

    private void HandleStartButtonPressed()
    {
        GD.Print("Start button pressed in repo");
    }

    private void HandleSettingsButtonPressed()
    {
        GD.Print("Settings button pressed in repo");
    }

    private void HandleExitButtonPressed()
    {
        GD.Print("Exit button pressed in repo");
    }
 }

// TO.Infras.Repositories/Nodes/UI/Screens/NodeLoadingScreenRepo.cs
using Autofac;
using Contexts;
using Godot;
using TO.Infras.Repositories.Bases;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace inFras.Nodes.UI.Screens;

public class NodeLoadingScreenRepo : NodeRepo<ILoadingScreen>
{
    public NodeLoadingScreenRepo(ILoadingScreen loadingScreen)
    {
        ContextEvents.TriggerRegisterNode(nameof(NodeLoadingScreenRepo), this, ConfigureNodeScope);
        Node = loadingScreen;
        Register();
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        // 连接加载屏幕特定的事件
        GD.Print("LoadingScreen repo connected");
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        // 断开加载屏幕特定的事件
        GD.Print("LoadingScreen repo disconnected");
    }

    public void UpdateProgress(float progress)
    {
        if (Node?.ProgressBar != null)
        {
            Node.ProgressBar.Value = progress;
        }
    }
}
 ```

#### 5. **具体仓储实现**

##### 表示层仓储实现
```csharp
// 表示层：主菜单屏幕仓储
public class MainMenuScreenRepo : NodeRepo<IMainMenuScreen>, IMainMenuScreenRepo
{
    public event Action? StartButtonPressed;
    public void EmitStartButtonPressed() => StartButtonPressed?.Invoke();
    
    public event Action? SettingsButtonPressed;
    public void EmitSettingsButtonPressed() => SettingsButtonPressed?.Invoke();
    
    public event Action? ExitButtonPressed;
    public void EmitExitButtonPressed() => ExitButtonPressed?.Invoke();
    
    public new IMainMenuScreen Node { get; }
    
    public MainMenuScreenRepo(IMainMenuScreen mainMenuScreen)
    {
        ContextEvents.TriggerRegisterNode(nameof(MainMenuScreenRepo), this, ConfigureNodeScope);
        Node = mainMenuScreen;
        Register();
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        if (Node.StartButton != null) Node.StartButton.Pressed += EmitStartButtonPressed;
        if (Node.SettingsButton != null) Node.SettingsButton.Pressed += EmitSettingsButtonPressed;
        if (Node.ExitButton != null) Node.ExitButton.Pressed += EmitExitButtonPressed;
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        if (Node.StartButton != null) Node.StartButton.Pressed -= EmitStartButtonPressed;
        if (Node.SettingsButton != null) Node.SettingsButton.Pressed -= EmitSettingsButtonPressed;
        if (Node.ExitButton != null) Node.ExitButton.Pressed -= EmitExitButtonPressed;
    }
}
```

##### 应用层仓储实现
```csharp
// 应用层：主菜单服务节点仓储
public class NodeMainMenuScreenRepo : NodeRepo<INodeMainMenuScreen>, INodeMainMenuScreenRepo
{
    public event Action? GameStartRequested;
    public void EmitGameStartRequested() => GameStartRequested?.Invoke();
    
    public event Action? SettingsRequested;
    public void EmitSettingsRequested() => SettingsRequested?.Invoke();
    
    public new INodeMainMenuScreen Node { get; }
    private readonly ICommandHandler _commandHandler;
    
    public NodeMainMenuScreenRepo(INodeMainMenuScreen nodeMainMenuScreen, ICommandHandler commandHandler)
    {
        ContextEvents.TriggerRegisterNode(nameof(NodeMainMenuScreenRepo), this, ConfigureNodeScope);
        Node = nodeMainMenuScreen;
        _commandHandler = commandHandler;
        Register();
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        // 连接应用层特定的事件
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        // 断开应用层特定的事件
    }
    
    public async Task HandleStartGameAsync()
    {
        await _commandHandler.HandleAsync(new StartGameCommand());
        EmitGameStartRequested();
    }
}
```

##### 基础设施层仓储实现
```csharp
// 基础设施层：数据持久化节点仓储
public class NodeDataPersistenceRepo : NodeRepo<INodeDataPersistence>, INodeDataPersistenceRepo
{
    public event Action<bool>? DataSaved;
    public void EmitDataSaved(bool success) => DataSaved?.Invoke(success);
    
    public event Action<GameData?>? DataLoaded;
    public void EmitDataLoaded(GameData? data) => DataLoaded?.Invoke(data);
    
    public new INodeDataPersistence Node { get; }
    private readonly IDataRepository _dataRepository;
    
    public NodeDataPersistenceRepo(INodeDataPersistence nodeDataPersistence, IDataRepository dataRepository)
    {
        ContextEvents.TriggerRegisterNode(nameof(NodeDataPersistenceRepo), this, ConfigureNodeScope);
        Node = nodeDataPersistence;
        _dataRepository = dataRepository;
        Register();
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        // 连接基础设施层特定的事件
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        // 断开基础设施层特定的事件
    }
    
    public async Task<bool> PersistGameDataAsync(GameData data)
    {
        var result = await _dataRepository.SaveAsync(data);
        EmitDataSaved(result);
        return result;
    }
    
    public async Task<GameData?> LoadGameDataAsync()
    {
        var data = await _dataRepository.LoadAsync();
        EmitDataLoaded(data);
        return data;
    }
}
```

### （三）生命周期管理机制


1. **节点创建**：

    - 节点实例化时从全局容器创建新的生命周期作用域
    - 在作用域内注册节点特定的服务实现
2. **依赖解析**：

    - 所有依赖通过构造函数注入
    - 仓储在自身作用域内注册内部依赖
3. **资源释放**：

    - 节点退出树时触发作用域释放
    - Autofac自动处理所有IDisposable对象的释放

## 三、优势分析

### （一）Autofac带来的额外价值


1. **高级生命周期管理**：

    - `InstancePerLifetimeScope`：确保每个作用域内使用同一个实例
    - `InstancePerDependency`：为每个请求创建新实例
2. **复杂依赖关系处理**：

    - 自动解析循环依赖
    - 支持命名和键控服务
3. **性能优化**：

    - 基于表达式树的快速服务解析
    - 作用域缓存机制减少重复创建

### （二）与Godot集成的优势


1. **无缝节点集成**：

    - 通过基类设计简化集成过程
    - 与Godot节点生命周期完美结合
2. **依赖隔离**：

    - 每个节点拥有独立的依赖环境
    - 避免全局状态导致的问题
3. **可测试性提升**：

    - 易于替换具体实现为测试双
    - 支持创建隔离的测试容器

## 四、实施建议

### （一）命名规范实施指南

#### 1. **项目结构组织**
```
demo/                           # 表示层（主项目）
├── Scripts/
│   ├── UI/
│   │   ├── MainMenuScreen.cs   # 表示层UI节点
│   │   └── GameplayScreen.cs
│   └── Game/
│       ├── Player.cs           # 表示层游戏节点
│       └── Enemy.cs

TO.Apps.Services/               # 应用层
├── Node/
│   ├── UI/
│   │   ├── NodeMainMenuScreen.cs    # 应用层UI服务节点
│   │   └── NodeGameplayScreen.cs
│   └── Game/
│       ├── NodePlayerController.cs  # 应用层游戏服务节点
│       └── NodeEnemyController.cs

TO.Infras.Repositories/         # 基础设施层
├── Node/
│   ├── Data/
│   │   ├── NodeDataPersistence.cs   # 基础设施层数据节点
│   │   └── NodeCacheManager.cs
│   └── Network/
│       ├── NodeNetworkClient.cs     # 基础设施层网络节点
│       └── NodeApiClient.cs
```

#### 2. **接口定义规范**
```csharp
// 表示层接口（TO.Nodes.Abstractions中定义）
// TO.Nodes.Abstractions/Nodes/UI/Screens/IMainMenuScreen.cs
using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Screens;

public interface IMainMenuScreen : INode
{
	public Button? StartButton { get; protected set; }
	
	public Button? SettingsButton { get; protected set; }
	
	public Button? ExitButton { get; protected set; }
	
	Action? OnStartButtonPressed { get; set; }
	
	Action? OnSettingsButtonPressed { get; set; }
	
	Action? OnExitButtonPressed { get; set; }
}

// 表示层接口（TO.Nodes.Abstractions中定义）
// TO.Nodes.Abstractions/Nodes/UI/Screens/ILoadingScreen.cs
using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Screens;

public interface ILoadingScreen : INode
{
    ProgressBar ProgressBar { get; set; }
}
```

#### 3. **仓储接口规范**
```csharp
// 表示层仓储接口
public interface IMainMenuScreenRepo : INodeRepo<IMainMenuScreen>
{
    event Action? StartButtonPressed;
    event Action? SettingsButtonPressed;
    event Action? ExitButtonPressed;
}

// 应用层仓储接口
public interface INodeMainMenuScreenRepo : INodeRepo<INodeMainMenuScreen>
{
    event Action? GameStartRequested;
    event Action? SettingsRequested;
    Task HandleStartGameAsync();
    Task HandleSettingsAsync();
}

// 基础设施层仓储接口
public interface INodeDataPersistenceRepo : INodeRepo<INodeDataPersistence>
{
    event Action<bool>? DataSaved;
    event Action<GameData?>? DataLoaded;
    Task<bool> PersistGameDataAsync(GameData data);
    Task<GameData?> LoadGameDataAsync();
}
```

### （二）最佳实践

#### 1. **架构设计原则**
- **遵循单一职责原则**：每个节点和服务应专注于单一功能
- **优先使用构造函数注入**：明确声明依赖关系
- **限制全局注册**：尽量将服务注册限制在需要的作用域内
- **使用命名作用域**：为不同类型的节点创建专用的作用域类型

#### 2. **命名规范执行**
- **严格遵循前缀规则**：应用层和基础设施层必须使用 `Node` 前缀
- **保持命名一致性**：同一业务概念在不同层级使用相同的核心名称
- **避免缩写**：使用完整的、有意义的名称
- **使用英文命名**：保持代码的国际化兼容性

#### 3. **依赖注入最佳实践**
- **接口优先**：始终依赖接口而非具体实现
- **生命周期管理**：合理选择服务的生命周期范围
- **循环依赖避免**：通过重构消除循环依赖
- **测试友好**：设计易于单元测试的依赖结构

### （三）性能考虑

#### 1. **节点创建优化**
```csharp
// 优化前：每次都创建新的服务实例
public class NodePlayerController : Node, INodePlayerController
{
    private readonly IPlayerService _playerService;
    private readonly IInputService _inputService;
    private readonly IPhysicsService _physicsService;
    
    public NodePlayerController(
        IPlayerService playerService,
        IInputService inputService,
        IPhysicsService physicsService)
    {
        _playerService = playerService;
        _inputService = inputService;
        _physicsService = physicsService;
    }
}

// 优化后：使用延迟初始化和缓存
public class NodePlayerController : Node, INodePlayerController
{
    private readonly Lazy<IPlayerService> _playerService;
    private readonly Lazy<IInputService> _inputService;
    private readonly Lazy<IPhysicsService> _physicsService;
    
    public NodePlayerController(
        Lazy<IPlayerService> playerService,
        Lazy<IInputService> inputService,
        Lazy<IPhysicsService> physicsService)
    {
        _playerService = playerService;
        _inputService = inputService;
        _physicsService = physicsService;
    }
    
    public override void _Ready()
    {
        // 只在需要时初始化服务
        if (IsPlayerActive)
        {
            _playerService.Value.Initialize();
        }
    }
}
```

#### 2. **生命周期管理策略**
- **避免过度注入**：不要为简单的节点注入过多的服务
- **合理使用生命周期**：根据实际需要选择合适的生命周期范围
- **延迟初始化**：对于重量级服务，考虑使用延迟初始化
- **内存管理**：及时释放不再需要的服务和节点

#### 3. **性能监控指标**
```csharp
public class NodePerformanceMonitor : Node, INodePerformanceMonitor
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, TimeSpan> _creationTimes = new();
    
    public void RecordNodeCreation(string nodeType, TimeSpan creationTime)
    {
        _creationTimes[nodeType] = creationTime;
        
        if (creationTime > TimeSpan.FromMilliseconds(100))
        {
            _logger.LogWarning($"Slow node creation: {nodeType} took {creationTime.TotalMilliseconds}ms");
        }
    }
    
    public void GeneratePerformanceReport()
    {
        var slowNodes = _creationTimes
            .Where(kvp => kvp.Value > TimeSpan.FromMilliseconds(50))
            .OrderByDescending(kvp => kvp.Value);
            
        foreach (var (nodeType, time) in slowNodes)
        {
            _logger.LogInformation($"Performance: {nodeType} - {time.TotalMilliseconds}ms");
        }
    }
}
```

### （四）调试与监控

#### 1. **依赖注入调试**
```csharp
public class NodeDependencyTracker : Node, INodeDependencyTracker
{
    private readonly ILogger _logger;
    private readonly Dictionary<Type, List<Type>> _dependencies = new();
    
    public void TrackDependency(Type nodeType, Type dependencyType)
    {
        if (!_dependencies.ContainsKey(nodeType))
        {
            _dependencies[nodeType] = new List<Type>();
        }
        
        _dependencies[nodeType].Add(dependencyType);
        _logger.LogDebug($"Node {nodeType.Name} depends on {dependencyType.Name}");
    }
    
    public void DetectCircularDependencies()
    {
        // 检测循环依赖的逻辑
        foreach (var (nodeType, deps) in _dependencies)
        {
            if (HasCircularDependency(nodeType, deps, new HashSet<Type>()))
            {
                _logger.LogError($"Circular dependency detected for {nodeType.Name}");
            }
        }
    }
}
```

#### 2. **生命周期监控**
```csharp
public class NodeLifecycleMonitor : Node, INodeLifecycleMonitor
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, DateTime> _nodeCreationTimes = new();
    
    public void OnNodeCreated(string nodeId, Type nodeType)
    {
        _nodeCreationTimes[nodeId] = DateTime.UtcNow;
        _logger.LogInformation($"Node created: {nodeType.Name} (ID: {nodeId})");
    }
    
    public void OnNodeDestroyed(string nodeId, Type nodeType)
    {
        if (_nodeCreationTimes.TryRemove(nodeId, out var creationTime))
        {
            var lifetime = DateTime.UtcNow - creationTime;
            _logger.LogInformation($"Node destroyed: {nodeType.Name} (ID: {nodeId}, Lifetime: {lifetime.TotalSeconds:F2}s)");
        }
    }
    
    public void GenerateLifecycleReport()
    {
        var activeNodes = _nodeCreationTimes.Count;
        var averageLifetime = CalculateAverageLifetime();
        
        _logger.LogInformation($"Active nodes: {activeNodes}, Average lifetime: {averageLifetime:F2}s");
    }
}
```

#### 3. **最佳实践清单**
- **日志记录**：在关键的生命周期事件中添加日志
- **性能监控**：监控服务创建和销毁的性能
- **依赖关系可视化**：使用工具可视化复杂的依赖关系
- **单元测试**：为每个服务和节点编写单元测试
- **集成测试**：测试节点与 Autofac 容器的集成
- **内存泄漏检测**：定期检查是否存在内存泄漏
- **错误处理**：实现健壮的错误处理和恢复机制

## 五、总结

### （一）方案核心价值

这种基于Autofac的倒置节点层设计为Godot C#项目带来了以下核心价值：

#### 1. **架构清晰性**
- **分层明确**：表示层、应用层、基础设施层职责清晰
- **依赖倒置**：高层模块不依赖低层模块，都依赖于抽象
- **接口驱动**：通过接口定义契约，提高代码的可测试性和可维护性

#### 2. **命名规范统一**
- **Node前缀规则**：应用层和基础设施层节点类统一使用`Node`前缀
- **接口命名一致**：接口名称与实现类名称保持对应关系
- **项目结构标准化**：不同层级的项目结构遵循统一的组织方式

#### 3. **生命周期精确控制**
- **自动化管理**：Autofac自动管理服务和节点的创建与销毁
- **作用域隔离**：不同类型的节点使用独立的作用域，避免相互干扰
- **资源优化**：及时释放不再需要的资源，提高内存使用效率

#### 4. **开发效率提升**
- **依赖注入**：自动解析和注入依赖，减少手动管理的复杂性
- **模块化设计**：各层独立开发，支持并行开发和测试
- **代码复用**：通过接口抽象，提高代码的复用性

### （二）实施关键要点

#### 1. **严格遵循命名规范**
```csharp
// ✅ 正确的命名方式
public class NodePlayerController : Node, INodePlayerController  // 应用层
public class NodeDataPersistence : Node, INodeDataPersistence    // 基础设施层
public class MainMenuScreen : Control, IMainMenuScreen           // 表示层

// ❌ 错误的命名方式
public class PlayerController : Node, IPlayerController          // 应用层缺少Node前缀
public class DataPersistence : Node, IDataPersistence            // 基础设施层缺少Node前缀
```

#### 2. **合理设计依赖关系**
- **单向依赖**：确保依赖关系是单向的，避免循环依赖
- **接口隔离**：接口应该小而专一，避免过于庞大的接口
- **依赖最小化**：只注入真正需要的依赖，避免过度注入

#### 3. **性能优化策略**
- **延迟初始化**：对于重量级服务使用`Lazy<T>`进行延迟初始化
- **生命周期选择**：根据实际需求选择合适的生命周期范围
- **监控和调试**：建立完善的监控和调试机制

### （三）未来扩展方向

#### 1. **工具链完善**
- **代码生成器**：开发自动生成节点类和接口的工具
- **依赖分析器**：可视化依赖关系图的分析工具
- **性能分析器**：专门针对节点生命周期的性能分析工具

#### 2. **框架集成**
- **MediatR集成**：与命令查询责任分离（CQRS）模式深度集成
- **事件系统**：建立基于事件的松耦合通信机制
- **配置管理**：支持动态配置和热重载功能

#### 3. **测试支持**
- **Mock框架**：为节点测试提供专门的Mock支持
- **集成测试**：建立端到端的集成测试框架
- **性能测试**：自动化的性能回归测试

这种基于Autofac的倒置节点层设计能够充分利用Autofac的强大功能，结合严格的命名规范和最佳实践，实现Godot C#项目中依赖关系的高效管理和资源的精确控制，为大型游戏项目的开发提供坚实的架构基础。
