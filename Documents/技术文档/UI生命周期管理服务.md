
## 功能概述

UILifecycleService提供了完整的UI组件生命周期管理，包括：


- 创建和销毁UI实例
- 资源缓存和复用
- 引用计数管理
- 内存优化

## 基础使用

### 1. 资源预加载


```csharp
// 预加载常用UI
_uiLifecycle.PreloadUI(new[]
{
    "res://UI/MainMenu.tscn",
    "res://UI/Inventory.tscn",
    "res://UI/Shop.tscn"
});


```

### 2. 销毁UI


```csharp
public void CloseInventory(IUIScreen inventory)
{
    _uiLifecycle.DestroyUI(
        inventory,
        "res://UI/Inventory.tscn");
}


```

## 高级功能

### 1. 资源状态监控


```csharp
public void LogUIStatus()
{
    var status = _uiLifecycle.GetUIStatus();
    foreach (var (path, info) in status)
    {
        GD.Print($"UI: {path}");
        GD.Print($"- Loaded: {info.IsLoaded}");
        GD.Print($"- References: {info.ReferenceCount}");
    }
}


```

### 2. 内存优化


```csharp
public void OptimizeMemory()
{
    // 清理未使用的UI资源
    _uiLifecycle.CleanupUnusedUI();
}


```

## 最佳实践


1. **预加载关键UI**


```csharp
   // 在游戏启动时预加载
   public override void _Ready()
   {
       _uiLifecycle.PreloadUI(GetCriticalUIPaths());
   }


```


1. **场景切换优化**

```csharp
public void OnLevelChange()
{
    // 清理不需要的UI
    _uiLifecycle.CleanupUnusedUI();
    // 预加载新场景UI
    _uiLifecycle.PreloadUI(GetNextLevelUIPaths());
}


```
2. **内存管理**

```csharp
public void OnLowMemory()
{
    // 主动清理未使用资源
    _uiLifecycle.CleanupUnusedUI();
}


```

## 注意事项


1. 确保正确调用DestroyUI以避免内存泄漏
2. 合理使用预加载功能，避免过度预加载
3. 定期检查UI状态，及时清理未使用资源
4. 与UIManagerService配合使用以获得完整的UI管理功能

## 与其他服务的集成

### 1. 与UIManagerService配合


```


```

sharp
public class UIController
{
private readonly IUILifecycleService _lifecycle;
private readonly IUIManagerService _manager;


```null
public async Task ShowScreen(string path) { var screen = _lifecycle.CreateUI(path, this); await _manager.ShowScreen(screen); }

```

}


```

### 2. 与UILayerService配合


```

csharp
public class UILayerController
{
private readonly IUILifecycleService _lifecycle;
private readonly IUILayerService _layer;


```null
public void ShowInLayer(string path, UILayer layer) { var parent = _layer.GetLayerNode(layer); var screen = _lifecycle.CreateUI(path, parent); }

```

}


```


```
