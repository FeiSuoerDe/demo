


# MagicFarmTales UI框架使用指南

## 1. 快速开始


```csharp
// 显示主菜单
var uiManager = GetNode<IUIManagerService>("/root/UIManager");
uiManager.Show("MainMenuScreen");


```

## 2. 创建新屏幕


1. 创建继承`UIScreen`的脚本


```csharp
[GlobalClass]
public partial class NewScreen : UIScreen 
{
    [Export] public Button MyButton { get; set; }
}


```


1. 在UIManager中添加引用


```csharp
[Export] public UIScreen? NewScreen { get; set; }


```

## 3. 屏幕导航

方法 | 描述
:----------- | :-----------
`Show(string name)` | 显示指定名称的屏幕
`CloseScreen()` | 返回上一屏幕
`HideScreens()` | 隐藏所有屏幕


## 4. 层级配置


```csharp
// 修改弹出层行为
UILayerConfig.UpdateLayerBehavior(UILayer.Popup, true);


```

## 5. 常见问题

**Q: 如何获取当前屏幕?**


```csharp
var current = _uiManagerRepo.CurrentScreen;


```

## 概述

UI系统提供了一套完整的界面管理解决方案，包括：


- 屏幕显示与隐藏管理
- 屏幕历史记录管理
- 屏幕层级关系管理
- 屏幕数据存储与查询
- 层级显示规则配置

系统由以下主要组件构成：


1. `IUIManagerService` - UI管理服务
2. `IUIManagerRepo` - UI数据存储仓库
3. `IUILayerService` - UI层级管理服务
4. `UILayerConfig` - UI层级配置管理

## 枚举定义

### UILayer

UI层级枚举，定义不同UI屏幕的显示层级和显示规则：

层级 | 值 | 描述
:----------- | :----------- | :-----------
Background | 0 | 背景层级，显示时不影响其他层
Normal | 100 | 普通层级，显示时隐藏下层
Dialog | 200 | 对话框层级，显示时不影响下层
Popup | 300 | 弹出层级，显示时不影响下层
Loading | 400 | 加载层层级，显示时不影响下层
Alert | 500 | 警告层级，显示时隐藏所有下层
System | 600 | 系统层级，显示时隐藏所有下层


## 接口定义

### IUIManagerService (UI管理服务接口)

#### 方法

#### `void Show(string screenName, bool keepInHistory = true)`

显示指定名称的屏幕

**参数**:


- `screenName` (string): 要显示的屏幕名称
- `keepInHistory` (bool): 是否保存到历史记录，默认为true

#### `void CloseScreen()`

关闭当前屏幕，显示历史记录中的上一个屏幕

#### `void HideScreens()`

隐藏所有屏幕并清空历史记录

### IUIManagerRepo (UI管理仓储接口)

#### 属性

#### `Dictionary<string, IUIScreen> ScreensByName`

按名称存储的UI屏幕字典

#### `Dictionary<UILayer, List<IUIScreen>> ScreensByLayer`

按层级存储的UI屏幕字典

#### `Stack<IUIScreen?>? History`

屏幕历史记录堆栈

#### `IUIScreen? CurrentScreen`

当前显示的屏幕

#### 方法

#### `void RegisterScreen(IUIScreen? screen)`

注册UI屏幕

**参数**:


- `screen` (IUIScreen): 要注册的屏幕对象

#### `IReadOnlyList<IUIScreen> GetScreensInLayer(UILayer layer)`

获取指定层级的所有屏幕

**参数**:


- `layer` (UILayer): 要查询的UI层级

**返回**:


- 该层级的所有屏幕只读列表

#### `IUIScreen? GetScreenByName(string name)`

根据名称获取屏幕

**参数**:


- `name` (string): 屏幕名称

**返回**:


- 找到的屏幕对象，如果不存在则返回null

### IUILayerService (UI层级管理服务接口)

#### 方法

#### `bool ShouldHideLowerLayers(UILayer layer)`

检查指定层级是否应该隐藏低层级UI

**参数**:


- `layer` (UILayer): 要检查的层级

**返回**:


- 是否应该隐藏低层级

#### `void ShowLayer(UILayer layer)`

显示指定层级的所有UI

**参数**:


- `layer` (UILayer): 要显示的层级

#### `void HideLayer(UILayer layer)`

隐藏指定层级的所有UI

**参数**:


- `layer` (UILayer): 要隐藏的层级

#### `void SetLayerVisible(UILayer layer, bool visible)`

设置指定层级的可见性

**参数**:


- `layer` (UILayer): 目标层级
- `visible` (bool): 是否可见

#### `void UpdateLayerZIndex(UILayer layer)`

更新指定层级的Z索引

**参数**:


- `layer` (UILayer): 要更新的层级

#### `void UpdateAllScreensZIndex()`

更新所有可见屏幕的Z索引

#### `void HandleScreenLayerRelation(IUIScreen newScreen, IUIScreen? currentScreen)`

处理屏幕层级关系

**参数**:


- `newScreen` (IUIScreen): 新显示的屏幕
- `currentScreen` (IUIScreen): 当前屏幕（可为null）

## 配置类

### UILayerConfig (UI层级配置)

#### 静态方法

#### `static bool ShouldHideLowerLayers(UILayer layer)`

获取指定层级是否应该隐藏下层UI

**参数**:


- `layer` (UILayer): 要检查的层级

**返回**:


- `true`表示应该隐藏下层UI，`false`表示不影响下层UI

#### `static void UpdateLayerBehavior(UILayer layer, bool hideLowerLayers)`

更新层级行为配置

**参数**:


- `layer` (UILayer): 要更新的层级
- `hideLowerLayers` (bool): 是否隐藏下层UI

#### `static void ResetToDefault()`

重置为默认配置

#### 默认层级行为规则

层级 | 隐藏下层
:----------- | :-----------
System | 是
Alert | 是
Loading | 否
Popup | 否
Dialog | 否
Normal | 是
Background | 否


### LayerBehavior (层级行为配置类)

#### 属性

#### `bool HideLowerLayers`

是否隐藏下层UI
