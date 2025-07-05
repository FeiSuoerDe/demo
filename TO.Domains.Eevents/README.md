# TO.Domains.Events - 领域事件

本项目包含了游戏中各种领域事件的定义。

## 文件结构

```
TO.Domains.Eevents/
├── Core/                    # 核心系统事件
│   └── SceneEvents.cs      # 场景相关事件（加载进度等）
├── Game/                    # 游戏逻辑事件
│   ├── FarmEvents.cs       # 农田系统事件
│   └── TimeEvents.cs       # 时间系统事件
└── README.md               # 本文档
```

## 事件分类

### Core 核心事件
- **SceneEvents**: 场景管理相关事件
  - `LoadingProgress`: 加载进度事件

### Game 游戏事件
- **FarmEvents**: 农田系统相关事件
  - `OnFarmlandWaterLevelChanged`: 农田存水量变化事件
  - `OnFarmlandWatered`: 农田浇水事件
  - `OnFarmlandDried`: 农田干涸事件
  - `OnFarmlandFullyWatered`: 农田达到最大存水量事件

- **TimeEvents**: 时间系统相关事件
  - `OnTimeUpdated`: 时间更新事件
  - `OnSeasonChanged`: 季节变化事件

## 使用方式

所有事件都实现了 `IEvent` 接口，可以通过事件总线进行发布和订阅：

```csharp
// 发布事件
eventBus.Publish(new OnFarmlandWaterLevelChanged(
    tilePosition: new Vector2I(10, 5),
    previousWaterLevel: 0.5f,
    currentWaterLevel: 0.8f,
    maxWaterLevel: 1.0f,
    isDry: false
));

// 订阅事件
eventBus.Subscribe<OnFarmlandWaterLevelChanged>(OnWaterLevelChanged);
```

## 命名约定

- 事件名称以 `On` 开头，使用过去时态
- 使用 record 类型定义事件，确保不可变性
- 为每个参数添加清晰的 XML 文档注释
- 按功能模块组织到不同的命名空间中