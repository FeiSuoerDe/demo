
## 概述

本UI框架基于Godot引擎实现，提供完整的UI生命周期管理、场景导航和事件处理功能。采用模块化设计，支持依赖注入和自动化的命令绑定机制。

## 整体架构


```
+---------------------+
|   Contexts (DI)     |
+----------+----------+
           |
+----------v----------+
|    NodeRegister     |
+----------+----------+
           |
+----------v----------+
|    UIManager        |
|  (IUIManager)       |
+----------+----------+
           |
+----------v----------+
|  UIScreen Base      |
+----------+----------+
           |
+----------v----------+
|  Specific Screens   |
|  (MainMenu, etc.)   |
+---------------------+


```

## 框架结构概览

整个框架由多个模块组成，包括：

模块 | 职责
:----------- | :-----------
[UIManager](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L11-L126) | 控制全局UI的显示与隐藏逻辑，继承自Godot`CanvasLayer`。
[IUIManagerService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services.Abstractions\UISystem\Managers\IUIManagerService.cs#L4-L10)/[UIManagerService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L6-L68) | 提供高层服务接口，封装UI管理行为。
[MainMenuScreenCommand](file://D:\GodotProjects\MagicFarmTales\TO.Apps.Commands\UI\Screens\MainMenuScreenCommand.cs#L8-L39) | 处理主菜单界面的按钮事件响应。
[MainMenuScreen](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L17-L17) | 主菜单UI的具体实现类，继承自基础类[UIScreen](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\Base\UIScreen.cs#L8-L41)。
[UIManagerRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Managers\UIManagerRepo.cs#L7-L34) | 数据仓库类，负责管理具体的UI屏幕实例及其状态。


## 核心组件详解

### [Contexts](file://D:\GodotProjects\MagicFarmTales\TO.Contexts\Contexts.cs#L8-L72)


```csharp
public class Contexts : LazySingleton<Contexts>


```


- 实现依赖注入容器
- 自动绑定`Repo`与`Command`的对应关系
- 提供全局注册接口`RegisterNode<T>()`

### [NodeRegister](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers.Abstractions\Nodes\NodeRegister.cs#L10-L34)


```csharp
public class NodeRegister


```


- 单例节点注册中心
- 支持不同类型的节点注册
- 当前仅实现
[IUIManager](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Managers\IUIManager.cs#L5-L9)
注册

### [UIManager](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L11-L26)


```csharp
public partial class UIManager : CanvasLayer, IUIManager


```


- UI系统的根节点
- 负责管理所有UI屏幕实例
- 实现
[IUIManager](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\UI\Managers\IUIManager.cs#L5-L9)
接口

### [UIScreen](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\Base\UIScreen.cs#L8-L41)


```csharp
public abstract partial class UIScreen : Control


```


- 所有UI界面的基类
- 提供标准显示/隐藏方法
- 支持动画效果

## 二、核心功能详解

### 1. UI 场景注册

在 

UIManagerRepo.cs

 中通过 

Screens

 列表统一管理所有UI组件，并在 

[RegisterScreens()](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L44-L51)

 方法中初始化：


```csharp
private void RegisterScreens()
  {
      MainMenuScreen = Singleton?.MainMenuScreen;
      SettingsMenuScreen = Singleton?.SettingsMenuScreen;
      Screens =
      [
          MainMenuScreen ?? throw new InvalidOperationException(),
          SettingsMenuScreen ?? throw new InvalidOperationException(),
      ];
  }

```

>

**说明**：使用 `[Export]` 特性绑定到 Godot 编辑器中的节点，确保 
[MainMenuScreen](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L17-L17)
 和 
[SettingsMenuScreen](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L18-L18)
 已正确赋值。

--- 


### 2. UI 显示与切换

#### (1) 高层接口定义


```csharp
public interface IUIManagerService
{
    void HideScreens();
    void ShowMainMenuScreen();
    void CloseScreen();
    void ShowSettingsMenuScreen();
}


```

#### (2) 实现类 [UIManagerService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L6-L68)


- 
[ShowMainMenuScreen()](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L28-L34)


```csharp
  public void ShowMainMenuScreen()
  {
      _uiManagerRepo.CurrentScreen = _uiManagerRepo.MainMenuScreen;
      HideScreens();
      _uiManagerRepo.History?.Push(_uiManagerRepo.MainMenuScreen);
      _uiManagerRepo.MainMenuScreen?.Show();
  }


```


- 
[ShowSettingsMenuScreen()](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L36-L39)

```csharp
public void ShowSettingsMenuScreen()
{
    Show(_uiManagerRepo.SettingsMenuScreen);
}


```
- `CloseScreen()`

```csharp
public void CloseScreen()
{
    if (_uiManagerRepo.HasUI)
    {	
        Show(_uiManagerRepo.History?.Pop(), false);
    }
}


```
- `Show(UIScreen?, bool)`

```csharp
private void Show(UIScreen? screen, bool keepInHistory = true)
{
    if (screen == null)
        return;

    if (_uiManagerRepo.CurrentScreen != null)
    {
        if (screen.IsTransparent)
            _uiManagerRepo.CurrentScreen.Hide();

        if (keepInHistory)
        {
            _uiManagerRepo.History?.Push(_uiManagerRepo.CurrentScreen);
        }
    }

    screen.Show();
    _uiManagerRepo.CurrentScreen = screen;
}


```

--- 


### 3. 屏幕切换逻辑


- 使用一个栈（`Stack<UIScreen?>`）来记录历史页面，支持返回上一级。
- 当前屏幕存储在 `_uiManagerRepo.CurrentScreen`。
- 支持透明屏幕叠加（如弹窗），不会影响背景页面的显示。

--- 


### 4. UI 生命周期管理


- 所有UI类继承自 `UIScreen`，并实现以下方法：

    - `_Ready()`：初始化资源或绑定事件。
    - `Show()`：显示UI。
    - `Hide()`：隐藏UI。
    - `OnDisable()`：销毁或清理资源。

## 功能模块

### 依赖注入系统

#### [UIModule](file://D:\GodotProjects\MagicFarmTales\TO.Contexts\UIModule.cs#L9-L35)


```csharp
builder.RegisterType<MainMenuScreenCommand>()
    .AsSelf()
    .InstancePerMatchingLifetimeScope(typeof(MainMenuScreenRepo));


```


- 注册UI相关服务
- 实现`Repo`与`Command`的生命周期绑定

#### [SingleModule](file://D:\GodotProjects\MagicFarmTales\TO.Contexts\SingleModule.cs#L7-L12)


```csharp
builder.RegisterType<UIManagerRepo>().As<IUIManagerRepo>().SingleInstance();


```


- 注册单例服务
- 管理全局状态

### UI服务层

#### [IUIManagerService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services.Abstractions\UISystem\Managers\IUIManagerService.cs#L5-L11)


```csharp
public interface IUIManagerService
{
    void ShowMainMenuScreen();
    void ShowSettingsMenuScreen();
    void CloseScreen();
}


```


- 定义UI操作接口
- 解耦UI和业务逻辑

#### [UIManagerService](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L7-L70)


```csharp
private void Show(UIScreen? screen, bool keepInHistory = true)


```


- 实现具体UI切换逻辑
- 维护界面栈状态
- 处理透明界面叠加情况

### 数据存储层

#### [IUIManagerRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers.Abstractions\Nodes\UI\Managers\IUIManagerRepo.cs#L7-L24)


```csharp
public interface IUIManagerRepo : ISingletonNodeRepo<IUIManager>
{
    UIScreen? MainMenuScreen { get; set; }
    UIScreen? SettingsMenuScreen { get; set; }
    Stack<UIScreen?>? History { get; set; }
}


```


- 定义UI状态存储结构
- 继承自
[ISingletonNodeRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers.Abstractions\Bases\ISingletonNodeRepo.cs#L7-L16)

#### [UIManagerRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Managers\UIManagerRepo.cs#L9-L54)


```csharp
public class UIManagerRepo : SingletonNodeRepo<IUIManager>, IUIManagerRepo


```


- 实现具体的UI状态存储
- 初始化时注册所有UI元素
- 维护界面历史记录

### 事件系统

#### [IMainMenuScreenRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers.Abstractions\Nodes\UI\Screens\IMainMenuScreenRepo.cs#L4-L10)


```csharp
public interface IMainMenuScreenRepo
{
    event Action? StartButtonPressed;
    event Action? SettingsButtonPressed;
}


```


- 定义UI事件接口
- 实现观察者模式

#### [MainMenuScreenCommand](file://D:\GodotProjects\MagicFarmTales\TO.Apps.Commands\UI\Screens\MainMenuScreenCommand.cs#L9-L75)


```csharp
public class MainMenuScreenCommand
{
    public MainMenuScreenCommand(IMainMenuScreenRepo mainMenuScreenRepo, IUIManagerService uiManagerService)
    {
        _mainMenuScreenRepo.StartButtonPressed += OnStartButtonPressed;
    }
}


```


- 事件处理器
- 实现具体业务逻辑
- 解耦UI和游戏逻辑

## 工作流程详解


1. **初始化阶段**

    - 创建
[Contexts](file://D:\GodotProjects\MagicFarmTales\TO.Contexts\Contexts.cs#L8-L72)
实例
    - 配置依赖注入容器
    - 注册
[NodeRegister](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers.Abstractions\Nodes\NodeRegister.cs#L10-L34)
2. **UI加载阶段**

    - 加载
[UIManager](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L11-L26)
节点
    - 通过`[Export]`属性加载所有UI场景
    - 注册到
[UIManagerRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Managers\UIManagerRepo.cs#L9-L54)
3. **事件绑定阶段**

    - 创建
[MainMenuScreenRepo](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Screens\MainMenuScreenRepo.cs#L9-L54)
    - 自动触发
[OnRegisterNodeRepo](file://D:\GodotProjects\MagicFarmTales\TO.Contexts\Contexts.cs#L39-L51)
事件
    - 解析并创建对应的
[MainMenuScreenCommand](file://D:\GodotProjects\MagicFarmTales\TO.Apps.Commands\UI\Screens\MainMenuScreenCommand.cs#L9-L75)
4. **运行时交互**

    - 用户点击按钮
    - 触发
[EmitStartButtonPressed()](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Screens\MainMenuScreenRepo.cs#L13-L13)
    - 调用
[OnStartButtonPressed()](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Screens\MainMenuScreenRepo.cs#L12-L12)
处理逻辑
    - 可能调用
[IUIManagerService.ShowSettingsMenuScreen()](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services.Abstractions\UISystem\Managers\IUIManagerService.cs#L9-L9)
切换界面

## 使用指南

### 添加新UI界面


1. 创建继承自
[UIScreen](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\Base\UIScreen.cs#L8-L41)
的新类


```csharp
public partial class NewScreen : UIScreen


```


1. 在
[UIManager](file://D:\GodotProjects\MagicFarmTales\Scripts\UI\Managers\UIManager.cs#L11-L26)
中添加导出属性


```csharp
[Export] public UIScreen? NewScreen { get; set; }


```


1. 创建对应的`INewScreenRepo`和`NewScreenRepo`


```csharp
public class NewScreenRepo : NodeRepo<INewScreen>, INewScreenRepo


```


1. 实现具体UI元素和交互逻辑

### 事件处理扩展


1. 在对应的`Repo`中定义事件


```csharp
public event Action? CustomButtonPressed;


```


1. 在
[ConnectNodeEvents()](file://D:\GodotProjects\MagicFarmTales\TO.Infras.Readers\Nodes\UI\Screens\MainMenuScreenRepo.cs#L27-L33)
中绑定事件


```csharp
if (Node.CustomButton != null) Node.CustomButton.Pressed += EmitCustomButtonPressed;


```


1. 在`Command`中订阅事件


```csharp
_mainMenuScreenRepo.CustomButtonPressed += OnCustomButtonPressed;


```

## 最佳实践


1. **保持UI和逻辑分离**

    - UI元素操作应在`Repo`中完成
    - 业务逻辑应在`Command`中实现
2. **合理使用界面栈**

    - 显示新界面时使用
[Show(screen, true)](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L47-L65)
    - 替换当前界面使用
[Show(screen, false)](file://D:\GodotProjects\MagicFarmTales\TO.Domains.Services\UISystem\Managers\UIManagerService.cs#L47-L65)
3. **资源管理**

    - 不要手动释放UI对象
    - 依赖框架自动管理生命周期
4. **错误处理**

    - 使用`GD.Print()`进行调试输出
    - 对关键操作添加异常捕获

## 注意事项


1. 所有UI元素必须继承自
[UIScreen](file://D:\GodotProjects\MagicFarmTales\TO.Nodes.Abstractions\Nodes\Base\UIScreen.cs#L8-L41)
2. 使用`[Export]`属性确保场景正确加载
3. 业务逻辑应放在对应的`Command`类中
4. 避免直接操作UI元素，应通过服务接口进行
5. 使用事件系统实现松耦合的交互
