
# UI动画系统 API 文档

## 核心功能

- **触发器系统**：支持多种UI事件触发（鼠标进入/离开、点击、双击等）
- **反应器系统**：响应触发器事件并执行动画效果
- **动画类型**：提供淡入淡出、缩放、移动等常用动画
- **参数配置**：支持持续时间、缓动类型、过渡类型等动画参数
- **生命周期管理**：自动管理动画资源和取消令牌

## 基本使用

### 服务获取

```csharp
// 获取UI动画服务
var animationService = serviceProvider.GetService<IUiAnimationService>();
```

### 触发器配置

```csharp
// 在场景中配置ObservableTrigger节点
[Export] public TriggerType TriggerType { get; set; } = TriggerType.MouseInOut;
[Export] public Control? TriggerControl { get; set; }
[Export] public bool TriggerOnReady { get; set; } = false;
```

### 反应器配置

```csharp
// 在场景中配置ObservableReactor节点
[Export] public Control? ReactControl { get; set; }
[Export] public ObservableTrigger? TriggerNode { get; set; }
[Export] public string FnName { get; set; } = "MouseInOut";
[Export] public Array<Variant> FnArgs { get; set; } = [];
[Export] public Tween.EaseType Ease { get; set; } = Tween.EaseType.Out;
[Export] public Tween.TransitionType Trans { get; set; } = Tween.TransitionType.Cubic;
```

## 触发器类型

### 支持的触发类型

```csharp
public enum TriggerType
{
    MouseIn,           // 鼠标进入
    MouseOut,          // 鼠标离开
    MouseInOut,        // 鼠标进入/离开
    Pressed,           // 按下
    Released,          // 释放
    PressedReleased,   // 按下/释放
    DoubleClicked,     // 双击
    Ready,             // 节点准备就绪
    VisibilityChanged  // 可见性改变
}
```

### 触发器使用示例

```csharp
// 鼠标进入/离开触发器
var trigger = new ObservableTrigger
{
    TriggerType = TriggerType.MouseInOut,
    TriggerControl = targetControl,
    TriggerOnReady = false
};
```

## 动画系统

### 预设动画方法

```csharp
// 鼠标进入/离开缩放动画
await animationService.MouseInOut(control, cancellationToken);

// 鼠标按下/释放渐变动画
await animationService.MousePressedReleased(control, cancellationToken);

// 弹出动画
await animationService.Popup(control, cancellationToken);

// 设置锚点偏移
animationService.SetPivotOffset(control, Vector2.Zero);
```

### 动画参数配置

#### 基础动画参数

```csharp
public record BaseAnimationParameters 
{
    public float Duration { get; init; } = 0.3f;                    // 动画持续时间
    public Tween.EaseType Ease { get; init; } = Tween.EaseType.Out; // 缓动类型
    public Tween.TransitionType Trans { get; init; } = Tween.TransitionType.Cubic; // 过渡类型
}
```

#### 淡入淡出动画参数

```csharp
var fadeParams = new FadeAnimationParameters
{
    Duration = 0.5f,
    FromAlpha = 0.0f,  // 起始透明度
    ToAlpha = 1.0f,    // 目标透明度
    Ease = Tween.EaseType.InOut,
    Trans = Tween.TransitionType.Sine
};
```

#### 缩放动画参数

```csharp
var scaleParams = new ScaleAnimationParameters
{
    Duration = 0.3f,
    FromScale = Vector2.One,      // 起始缩放
    ToScale = new Vector2(1.1f, 1.1f), // 目标缩放
    Ease = Tween.EaseType.Out,
    Trans = Tween.TransitionType.Back
};
```

#### 移动动画参数

```csharp
var moveParams = new MoveAnimationParameters
{
    Duration = 0.4f,
    FromPosition = Vector2.Zero,     // 起始位置
    ToPosition = new Vector2(100, 0), // 目标位置
    Ease = Tween.EaseType.InOut,
    Trans = Tween.TransitionType.Quart
};
```

## 动画上下文

### 创建动画上下文

```csharp
// 创建动画上下文
var context = new AnimationContext(control, parameters, cancellationToken);

// 使用上下文执行动画
context.Tween?.TweenProperty(control, "modulate:a", 1.0f, parameters.Duration);

// 清理资源
context.Dispose();
```

### 取消令牌管理

```csharp
// 设置取消令牌
animationService.SetupCancellationToken(control, cancellationToken);

// 动画会在令牌取消时自动停止
if (cancellationToken.IsCancellationRequested)
{
    return;
}
```

## 场景配置

### 在Godot场景中配置

1. **添加触发器节点**
   ```
   Control (目标控件)
   └── ObservableTrigger
       └── ObservableReactor
   ```

2. **配置触发器属性**
   - `TriggerType`: 选择触发类型
   - `TriggerControl`: 设置触发控件（通常为父节点）
   - `TriggerOnReady`: 是否在Ready时触发

3. **配置反应器属性**
   - `ReactControl`: 设置反应控件
   - `TriggerNode`: 关联触发器节点
   - `FnName`: 动画方法名称
   - `FnArgs`: 动画参数数组
   - `Ease`: 缓动类型
   - `Trans`: 过渡类型

## 最佳实践

### 动画性能优化

```csharp
// 推荐：使用预设动画方法
await animationService.MouseInOut(control, cancellationToken);

// 避免：频繁创建动画上下文
// 应该复用或使用对象池
```

### 资源管理

```csharp
// 确保动画上下文正确释放
using var context = new AnimationContext(control, parameters, cancellationToken);
// 动画执行代码
// context会在using块结束时自动释放
```

### 取消令牌使用

```csharp
// 为每个控件设置独立的取消令牌
var cts = new CancellationTokenSource();
animationService.SetupCancellationToken(control, cts.Token);

// 在适当时机取消动画
cts.Cancel();
```

## 注意事项

- **节点生命周期**：确保触发器和反应器节点在目标控件的子节点中
- **参数验证**：动画参数会自动验证，无效参数将导致动画失败
- **内存管理**：动画上下文实现了IDisposable，使用完毕后需要释放
- **线程安全**：动画操作应在主线程中执行
- **性能考虑**：避免同时运行过多动画，可能影响帧率
- **取消机制**：合理使用CancellationToken来控制动画生命周期
