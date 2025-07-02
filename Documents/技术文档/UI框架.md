
# UI框架使用指南

## 1. 核心功能

提供完整的UI管理服务，主要功能包括：

- UI屏幕的显示/隐藏管理
- 层级关系处理
- 生命周期管理
- 事件驱动的UI操作
- 智能屏幕切换

## 2. 基本使用

### 应用层服务获取

通过依赖注入获取UI服务实例：

```csharp
public class MyService(IUIManagerService uiManager, IUILayerService layerService)
{
    // 使用uiManager访问UI管理功能
}
```

### 显示UI屏幕

```csharp
// 通过事件显示UI
var eventBus = _eventBus[EventEnums.UI];
eventBus.Publish(new ShowUI(UIName.MainMenuScreen));

// 直接调用服务
uiManager.Show(screen);
```

### 隐藏UI屏幕

```csharp
// 隐藏当前屏幕
eventBus.Publish(new HideUI());

// 隐藏所有屏幕
eventBus.Publish(new HideAllUI());
```

### 关闭UI屏幕

```csharp
// 关闭指定屏幕
eventBus.Publish(new CloseUI(UIName.SettingsMenuScreen));

// 关闭所有屏幕
eventBus.Publish(new CloseAllUI());
```

## 3. 层级管理

### 层级类型

```csharp
public enum UILayerType
{
    Background = 0,
    Normal = 100,
    Dialog = 200, 
    Popup = 300,
    Loading = 400, 
    Alert = 500,
    System = 600,
}
```

### 层级操作

```csharp
// 显示指定层级
layerService.ShowLayer(UILayerType.Dialog);

// 隐藏指定层级
layerService.HideLayer(UILayerType.Popup);

// 设置层级可见性
layerService.SetLayerVisible(UILayerType.Normal, true);

// 处理屏幕显示时的层级关系
layerService.HandleShowScreenLayerRelation(newScreen, currentScreen);
```

## 4. 生命周期管理

### 创建UI

```csharp
// 推荐：使用UIConfigs中的路径配置
var path = UIConfigs.UIPaths[UIName.MainMenuScreen];
var screen = lifecycleService.CreateUI(path);

// 不推荐：直接使用硬编码路径
// var screen = lifecycleService.CreateUI("res://Scenes/UI/Screens/MainMenuScreen.tscn");
```

### 销毁UI

```csharp
// 推荐：使用UIConfigs中的路径配置
var path = UIConfigs.UIPaths[UIName.MainMenuScreen];
lifecycleService.DestroyUI(screen, path);

// 不推荐：直接使用硬编码路径
// lifecycleService.DestroyUI(screen, "res://Scenes/UI/Screens/MainMenuScreen.tscn");

// 销毁所有UI
lifecycleService.DestroyAllUI();
```

## 5. 配置管理

### UI路径配置

```csharp
public class UIConfigs
{
    public static Dictionary<UIName,string> UIPaths = new()
    {
        {UIName.MainMenuScreen, "res://Scenes/UI/Screens/MainMenuScreen.tscn"},
        {UIName.SettingsMenuScreen, "res://Scenes/UI/Screens/SettingsMenuScreen.tscn"},
        {UIName.VolumeSettingsScreen, "res://Scenes/UI/Screens/VolumeSettingsScreen.tscn"},
        {UIName.LoadingScreen, "res://Scenes/UI/Screens/LoadingScreen.tscn"}
    };
}
```

### UI名称枚举

```csharp
public enum UIName
{
    MainMenuScreen,
    SettingsMenuScreen,
    VolumeSettingsScreen,
    LoadingScreen
}
```

## 6. 事件系统

### 可用事件

```csharp
// 显示UI事件
public record ShowUI(UIName UIName) : IEvent;

// 隐藏UI事件
public record HideUI() : IEvent;

// 隐藏所有UI事件
public record HideAllUI() : IEvent;

// 关闭UI事件
public record CloseUI(UIName UIName) : IEvent;

// 关闭所有UI事件
public record CloseAllUI() : IEvent;
```

### 事件发布

```csharp
// 获取事件总线
var eventBus = _eventBus[EventEnums.UI];

// 发布显示事件
eventBus.Publish(new ShowUI(UIName.MainMenuScreen));

// 发布关闭事件
eventBus.Publish(new CloseUI(UIName.SettingsMenuScreen));
```

## 7. 屏幕管理

### 获取屏幕实例

```csharp
// 通过名称获取屏幕
var screen = uiManagerRepo.GetScreenByName("MainMenuScreen");

// 通过层级获取屏幕列表
var screens = uiManagerRepo.GetScreensInLayer(UILayerType.Dialog);
```

### 屏幕注册

```csharp
// 注册屏幕到仓库
uiManagerRepo.RegisterScreen(screenName, screen);

// 注销屏幕
uiManagerRepo.UnregisterScreen(screenName);
```

## 8. 最佳实践

1. **事件驱动优先**：
   - 优先使用事件系统进行UI操作
   - 保持组件间的松耦合

2. **层级管理**：
   - 合理设置UI层级类型
   - 利用自动层级处理功能

3. **资源管理**：
   - 及时销毁不再使用的UI
   - 使用DestroyAllUI进行批量清理

4. **配置集中化**：
   - 在UIConfigs中统一管理UI路径
   - 避免硬编码路径字符串
   - 调用方法时使用UIConfigs.UIPaths[UIName]获取路径

## 9. 注意事项

- 所有UI路径必须在UIConfigs中配置
- **避免硬编码路径**：调用UI相关方法时，应使用`UIConfigs.UIPaths[UIName]`获取路径，而不是直接写入路径字符串
- 使用事件系统时需要正确的EventEnums类型
- 销毁UI会同时清理资源引用计数
- 层级关系会自动处理，无需手动管理
- 屏幕切换会自动处理历史记录和层级关系
