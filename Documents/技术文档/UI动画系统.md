
本系统基于 Godot 引擎与 C# 实现，采用领域驱动设计（DDD）架构，构建一个完全解耦、事件驱动、支持多种触发方式的 UI 动画系统。以下为详细的技术实现说明。

--- 


## 一、系统结构概览

### 1. 分层架构


- **应用层（TO.Apps.Commands）**：负责将动画命令与具体事件绑定。
- **领域层（TO.Domains.Services）**：封装核心动画逻辑和触发服务。
- **基础设施层（TO.Infras.Readers）**：提供节点仓库实现与事件注册机制。
- **抽象接口层（TO.Nodes.Abstractions）**：定义统一的接口规范，确保模块间松耦合。
- **通用工具层（TO.Commons）**：提供扩展方法、枚举等基础功能。

### 2. 核心组件关系图


```
+-------------------+       +----------------------+        +--------------------------+
| ObservableTrigger |<----->| ObservableTriggerRepo |<------| ObservableTriggerService |
+-------------------+       +----------------------+        +--------------------------+
         |
         v
+-------------------+       +----------------------+        +-------------------------+
| ObservableReactor |<----->| ObservableReactorRepo |<------| ObservableReactorCommand |
+-------------------+       +----------------------+        +-------------------------+
                                                             |
                                                             v
                                                +---------------------------+
                                                |   IUiAnimationService     |
                                                | (Concrete: UiAnimationService) |
                                                +---------------------------+


```

--- 


## 二、关键节点配置与使用说明

### 1. `ObservableTrigger` 节点（[IObservableTrigger](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableTrigger.cs#L6-L15)）

#### 配置字段说明：

字段名 | 类型 | 描述
:----------- | :----------- | :-----------
`TriggerControl` | `Control?` | 绑定的目标控件，用于监听其输入或状态变化事件
`TriggerOnReady` | `bool` | 是否在控件就绪时自动触发一次动画
`TriggerType` | `TriggerType` | 触发类型，如按下、释放、鼠标进入/离开、双击、可见性变化等
`Triggered` | `Action<Dictionary<string, object>?>` | 触发事件回调，由`ObservableReactor`监听并执行动画


--- 


### 2. [ObservableReactor](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Trigger\ObservableReactor.cs#L10-L34) 节点（[IObservableReactor](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L6-L17)）

#### 配置字段说明：

字段名 | 类型 | 描述
:----------- | :----------- | :-----------
[ReactControl](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L8-L8) | `Control?` | 被操作的控件，即要执行动画的对象
[Trigger](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L9-L9) | `Node?` | 关联的[ObservableTrigger](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Trigger\ObservableTrigger.cs#L15-L36)节点
[FnName](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L11-L11) | `string` | 动画方法名，需与[IUiAnimationService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services.Abstractions\UISystem\UiAnimationService\IUiAnimationService.cs#L5-L39)接口中定义的方法名一致
[FnArgs](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L12-L12) | `Array<Variant>` | 动画参数数组，顺序与目标方法签名一致
[Ease](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L14-L14) | `Tween.EaseType` | 缓动函数类型
[Trans](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Trigger\IObservableReactor.cs#L16-L16) | `Tween.TransitionType` | 过渡类型


--- 


## 三、动画服务与命令模式

### 1. [IUiAnimationService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services.Abstractions\UISystem\UiAnimationService\IUiAnimationService.cs#L5-L39) / [UiAnimationService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L7-L141)

该接口封装了所有动画行为，包括：


- 
[Scale](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L24-L33)
：缩放动画
- 
[Move](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L35-L45)
：移动动画
- 
[MoveFrom](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L47-L56)
：从某位置移动过来
- 
[Visible](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L58-L67)
：透明度渐变显示/隐藏
- 
[MouseInOut](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L69-L87)
：根据鼠标进入/离开做缩放动画
- 
[MousePressedReleased](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L89-L109)
：根据按压状态改变透明度
- 
[Popup](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L111-L130)
：弹出/收回动画
- 
[Test](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\UiAnimationService\UiAnimationService.cs#L132-L135)
：测试方法，输出日志

每个方法接受 `Control`, `ease`, `trans`, `args`, `kwargs` 等参数，支持动态调用。

### 2. [ObservableReactorCommand](file://D:\GodotProjects\MagicFarmTales\TO.Apps.Commands\UI\Trigger\ObservableReactorCommand.cs#L8-L40)

通过反射调用动画方法，实现命令与动画逻辑解耦。


```csharp
public class ObservableReactorCommand : IDisposable
{
    private readonly IObservableReactorRepo _observableReactorRepo;
    private readonly IUiAnimationService _uiAnimationService;

    public ObservableReactorCommand(IObservableReactorRepo observableReactorRepo, IUiAnimationService uiAnimationService)
    {
        _observableReactorRepo = observableReactorRepo;
        _uiAnimationService = uiAnimationService;
        if (_observableReactorRepo.Trigger != null) _observableReactorRepo.Trigger.Triggered += Triggered;
    }

    private void Triggered(Dictionary<string, object>? data)
    {
        var methodInfo = _uiAnimationService.GetType().GetMethod(_observableReactorRepo.FnName);
        if (methodInfo == null) throw new Exception("没有找到该动画！");
        
        methodInfo.Invoke(_uiAnimationService,
            [_observableReactorRepo.ReactControl, _observableReactorRepo.Ease, 
             _observableReactorRepo.Trans, _observableReactorRepo.FnArgs, data]);
    }
}


```

--- 


## 四、生命周期管理与依赖注入

### 1. 生命周期控制


- 所有类实现 `IDisposable`，保证资源释放。
- 使用 
[NodeScope](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Trigger\ObservableTrigger.cs#L25-L25)
 管理局部上下文生命周期，避免全局污染。

### 2. 依赖注入（Autofac）


- 所有组件通过构造函数注入依赖项，符合依赖倒置原则。
- 例如：


```csharp
public class ObservableTriggerService : IObservableTriggerService
{
    private readonly IObservableTriggerRepo _observableTriggerRepo;

    public ObservableTriggerService(IObservableTriggerRepo observableTriggerRepo)
    {
        _observableTriggerRepo = observableTriggerRepo;
        Initialize();
        Register();
    }
}


```
