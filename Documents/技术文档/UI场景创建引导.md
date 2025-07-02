# UI场景创建引导

本文档提供了在项目中创建新UI场景的完整步骤指南，包括文件结构、代码模板和最佳实践。

## 目录结构概览

```
项目根目录/
├── Scenes/UI/Screens/                              # Godot场景文件 (.tscn)
├── Scripts/UI/Screens/                             # UI场景脚本实现
├── TO.Nodes.Abstractions/                          # 节点接口定义层
│   └── Nodes/UI/Screens/
├── TO.Domains.Models.Repositories.Abstractions/    # Repository接口层
│   └── Nodes/UI/Screens/
├── TO.Infras.Repositories/                         # Repository实现层
│   └── Nodes/UI/Screens/
└── TO.Apps.Services/                               # 业务逻辑服务层
    └── Node/UI/Screens/
```

## 创建步骤

### 1. 创建Godot场景文件

**位置**: `Scenes/UI/Screens/`

1. 在Godot编辑器中创建新场景
2. 设置根节点类型为 `Control`
3. 添加所需的UI控件（Button、Label等）
4. 保存为 `{ScreenName}Screen.tscn`

**示例**: `ExampleScreen.tscn`

### 2. 定义接口

**位置**: `TO.Nodes.Abstractions/Nodes/UI/Screens/`
**文件名**: `I{ScreenName}Screen.cs`

```csharp
using System;
using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Nodes.UI.Screens;

public interface IExampleScreen : INode
{
    // 导出的UI控件属性
    public Button? ActionButton { get; protected set; }
    public Label? TitleLabel { get; protected set; }
    
    // 事件回调
    Action? OnActionButtonPressed { get; set; }
}
```

### 3. 实现UI场景脚本

**位置**: `Scripts/UI/Screens/`
**文件名**: `{ScreenName}Screen.cs`

```csharp
using System;
using Contexts;
using Godot;
using inFras.Nodes.UI.Screens;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace demo.UI.Screens;

public partial class ExampleScreen : Bases.UIScreen, IExampleScreen
{
    [Export]
    public Button? ActionButton { get; set; }
    
    [Export]
    public Label? TitleLabel { get; set; }
    
    public Action? OnActionButtonPressed { get; set; }
    private void EmitActionButtonPressed() => OnActionButtonPressed?.Invoke();
    
    public override void _Ready()
    {
        base._Ready();
        
        // 绑定事件
        if (ActionButton != null) 
            ActionButton.Pressed += EmitActionButtonPressed;
            
        // 注册到依赖注入容器
        NodeScope = NodeContexts.Instance.RegisterNode<IExampleScreen, NodeExampleScreenRepo>(this);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        
        // 解绑事件
        if (ActionButton != null) 
            ActionButton.Pressed -= EmitActionButtonPressed;
    }
}
```

### 4. 创建Repository接口

**位置**: `TO.Domains.Models.Repositories.Abstractions/Nodes/UI/Screens/`
**文件名**: `I{ScreenName}ScreenRepo.cs`

```csharp
using System;

namespace TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;

public interface IExampleScreenRepo
{
    // 事件定义
    public event Action? ActionButtonPressed;
    
    // 如果有数据操作方法，也在这里定义
    // void SetSomeData(string data);
}
```

### 5. 实现Repository类

**位置**: `TO.Infras.Repositories/Nodes/UI/Screens/`
**文件名**: `Node{ScreenName}ScreenRepo.cs`

```csharp
using System;
using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Events.Contexts;
using TO.Nodes.Abstractions.Nodes.UI.Screens;

namespace inFras.Nodes.UI.Screens;

public class NodeExampleScreenRepo : NodeRepo<IExampleScreen>, IExampleScreenRepo
{
    public event Action? ActionButtonPressed;
    private void EmitActionButtonPressed() => ActionButtonPressed?.Invoke();
    
    public NodeExampleScreenRepo(IExampleScreen exampleScreen)
    {
        Node = exampleScreen;
        Register();
        ContextEvents.TriggerRegisterNode(this, ConfigureNodeScope);
    }

    protected override void ConnectNodeEvents()
    {
        base.ConnectNodeEvents();
        if (Node == null) return;
        Node.OnActionButtonPressed += EmitActionButtonPressed;
    }

    protected override void DisconnectNodeEvents()
    {
        base.DisconnectNodeEvents();
        if (Node == null) return;
        Node.OnActionButtonPressed -= EmitActionButtonPressed;
    }
    
    // 如果有数据操作方法的实现
    // public void SetSomeData(string data)
    // {
    //     Node?.SetSomeData(data);
    // }
}
```

### 6. 创建业务逻辑服务

**位置**: `TO.Apps.Services/Node/UI/Screens/`
**文件名**: `Node{ScreenName}ScreenService.cs`

```csharp
using Apps.Commands.Core;
using Autofac.Features.Indexed;
using Godot;
using MediatR;
using TO.Apps.Events;
using TO.Apps.Services.Abstractions.Bases;
using TO.Commons.Enums;
using TO.Domains.Models.Repositories.Abstractions.Core.EventBus;
using TO.Domains.Models.Repositories.Abstractions.Nodes.UI.Screens;
using TO.Domains.Services.Abstractions.Core.SceneSystem;

namespace TO.Apps.Services.Node.UI.Screens;

public class NodeExampleScreenService : BaseService
{
    private readonly IExampleScreenRepo _exampleScreenRepo;
    private readonly ISceneManagerService _sceneManagerService;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public NodeExampleScreenService(
        IExampleScreenRepo exampleScreenRepo,
        IIndex<EventEnums, IEventBus> eventBus,
        ISceneManagerService sceneManagerService,
        IMediator mediator)
    {
        _exampleScreenRepo = exampleScreenRepo;
        _sceneManagerService = sceneManagerService;
        _mediator = mediator;
        _eventBus = eventBus[EventEnums.UI];
       
        // 订阅事件
        _exampleScreenRepo.ActionButtonPressed += OnActionButtonPressed;
    }

    private void OnActionButtonPressed()
    {
        GD.Print("Action button pressed!");
        // 实现具体的业务逻辑
        // 例如：_mediator.Send(new SomeCommand());
        // 或者：_eventBus.Publish(new SomeEvent());
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        _exampleScreenRepo.ActionButtonPressed -= OnActionButtonPressed;
    }
}
```

### 7. 配置UI枚举和路径

#### 添加UI名称枚举

**文件**: `TO.Commons/Enums/UIName.cs`

```csharp
public enum UIName
{
    // ... 现有枚举值
    ExampleScreen,  // 添加新的UI名称
}
```

#### 配置UI路径

**文件**: `TO.Commons/Configs/UIConfigs.cs`

```csharp
public static readonly Dictionary<UIName, string> UIPaths = new()
{
    // ... 现有路径配置
    { UIName.ExampleScreen, "res://Scenes/UI/Screens/ExampleScreen.tscn" },
};
```

### 8. 绑定场景脚本

1. 在Godot编辑器中打开场景文件
2. 选择根节点
3. 在Inspector面板中，点击脚本图标
4. 选择 "Attach Script"
5. 选择已创建的脚本文件
6. 在Inspector中导出需要的UI控件

## 使用示例

### 显示UI场景

```csharp
// 通过事件系统
_eventBus.Publish(new ShowUI(UIName.ExampleScreen));

// 通过UI管理器服务
_uiManagerService.ShowUI(UIName.ExampleScreen);
```

### 隐藏UI场景

```csharp
// 隐藏特定UI
_eventBus.Publish(new HideUI(UIName.ExampleScreen));

// 隐藏所有UI
_eventBus.Publish(new HideAllUI());
```

### 关闭UI场景

```csharp
// 关闭特定UI
_eventBus.Publish(new CloseUI(UIName.ExampleScreen));

// 关闭所有UI
_eventBus.Publish(new CloseAllUI());
```

## 文件命名规范

| 文件类型 | 命名规范 | 示例 |
|---------|---------|------|
| 场景文件 | `{Name}Screen.tscn` | `ExampleScreen.tscn` |
| 脚本文件 | `{Name}Screen.cs` | `ExampleScreen.cs` |
| 节点接口 | `I{Name}Screen.cs` | `IExampleScreen.cs` |
| Repository接口 | `I{Name}ScreenRepo.cs` | `IExampleScreenRepo.cs` |
| Repository实现 | `Node{Name}ScreenRepo.cs` | `NodeExampleScreenRepo.cs` |
| 服务文件 | `Node{Name}ScreenService.cs` | `NodeExampleScreenService.cs` |

## 最佳实践

### 1. 事件处理
- 使用Action委托进行事件回调
- 在`_Ready()`中绑定事件，在`_ExitTree()`中解绑
- 避免在UI脚本中直接实现业务逻辑

### 2. 分层架构
- **节点层**: 继承UIScreen，处理UI控件和基础事件
- **Repository层**: 封装节点操作，提供业务友好的接口
- **Service层**: 实现具体业务逻辑，处理复杂交互
- **依赖注入**: 通过`NodeContexts.Instance.RegisterNode`自动注册

### 3. 依赖注入
- Repository自动注册到DI容器
- Service层通过构造函数注入Repository
- 利用Autofac的生命周期管理资源

### 4. 资源管理
- 继承自`UIScreen`基类获得自动资源管理
- 使用`NodeScope`管理依赖注入作用域
- 确保在`_ExitTree()`中清理资源

### 5. 层级管理
- 使用`UILayerType`枚举设置UI层级
- 通过`IsTransparent`属性控制透明度
- 合理使用`ZIndex`控制显示顺序

### 6. 配置管理
- 使用`UIConfigs.UIPaths`获取场景路径，避免硬编码
- 统一在`UIName`枚举中管理UI名称
- 保持配置的集中化管理

## 常见问题

### Q: 事件没有触发怎么办？
A: 检查以下几点：
- 确认UI控件已正确导出`[Export]`
- 检查`_Ready()`方法是否调用了`base._Ready()`
- 验证事件绑定和解绑是否正确

### Q: 依赖注入失败怎么办？
A: 检查以下几点：
- 确认相关服务已在DI容器中注册
- 检查接口和实现类的命名是否匹配
- 查看控制台是否有错误信息

### Q: UI显示异常怎么办？
A: 检查以下几点：
- 确认场景路径配置正确
- 检查UI层级设置是否合适
- 验证场景文件结构是否正确

## 架构层次说明

### 分层职责

1. **表现层 (Presentation Layer)**
   - **位置**: `Scripts/UI/Screens/`
   - **职责**: 继承UIScreen基类，处理UI控件绑定和基础事件转发
   - **特点**: 不包含业务逻辑，仅负责UI展示和事件传递

2. **基础设施层 (Infrastructure Layer)**
   - **位置**: `TO.Infras.Repositories/Nodes/UI/Screens/`
   - **职责**: 封装节点操作，提供统一的事件接口和数据操作方法
   - **特点**: 继承NodeRepo基类，自动管理节点生命周期和事件绑定

3. **应用服务层 (Application Service Layer)**
   - **位置**: `TO.Apps.Services/Node/UI/Screens/`
   - **职责**: 实现具体业务逻辑，协调不同服务和组件
   - **特点**: 通过依赖注入获取Repository，处理复杂的业务交互

4. **抽象层 (Abstraction Layer)**
   - **位置**: `TO.Nodes.Abstractions/` 和 `TO.Domains.Models.Repositories.Abstractions/`
   - **职责**: 定义接口契约，实现依赖倒置
   - **特点**: 确保各层之间的松耦合

### 数据流向

```
UI事件 → UIScreen → Repository → Service → 业务逻辑处理
                     ↓
                  事件发布 → 其他系统响应
```

## 总结

遵循本指南创建的UI场景将具有：
- **清晰的四层架构**: 表现层、基础设施层、应用服务层、抽象层
- **完整的Repository模式**: 封装节点操作，提供业务友好接口
- **自动的生命周期管理**: 通过NodeRepo基类和依赖注入容器
- **统一的事件处理机制**: 从UI控件到业务逻辑的完整事件链
- **强类型的接口约束**: 通过抽象层确保编译时类型安全
- **良好的可测试性**: 各层职责单一，依赖关系清晰

这种架构模式遵循DDD（领域驱动设计）原则，确保了代码的可维护性、可扩展性和团队协作的一致性。