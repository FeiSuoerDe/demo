# 属性集 SQLite 数据库系统

这是一个为 Godot 游戏项目设计的属性集数据库系统，用于存储和管理玩家的属性数据。

## 📁 项目结构

```
TO.Data/
├── Tables/
│   └── PlayerAttributeDatabase.sql     # 数据库初始化脚本
├── Models/GameAbilitySystem/Database/
│   ├── AttributeSetEntity.cs           # 属性集实体
│   ├── AttributeValueEntity.cs         # 属性值实体
│   ├── AttributeEffectEntity.cs        # 属性效果实体
│   └── AttributeModifierEntity.cs      # 属性修饰器实体
├── Database/
│   └── AttributeDbContext.cs           # EF Core 数据库上下文
├── Repositories/
│   ├── IAttributeSetRepository.cs      # 仓储接口
│   └── AttributeSetRepository.cs       # 仓储实现
├── Services/
│   ├── AttributeSetMappingService.cs   # 映射服务
│   └── AttributeSetDatabaseService.cs  # 数据库服务
├── Scripts/
│   └── InitializeDatabase.cs           # 数据库初始化脚本
├── Examples/
│   └── DatabaseExample.cs              # 使用示例
└── README.md                            # 说明文档
```

## 🗄️ 数据库设计

### 核心表结构

1. **AttributeSets** - 属性集表
   - 存储玩家的属性集基本信息
   - 支持一个玩家拥有多个属性集

2. **AttributeValues_BasicAttributes** - 基础属性值表
   - 存储基础属性值（生命值、能量值、力量、敏捷等）
   - 包含基础值、当前值、最小值、最大值

3. **AttributeValues_ShipAttributes** - 飞船属性值表
   - 存储飞船属性值（推进力、护盾、装甲等）
   - 包含基础值、当前值、最小值、最大值

4. **AttributeEffects** - 属性效果表
   - 存储应用到属性集的效果
   - 支持临时效果、永久效果、堆叠效果

5. **AttributeModifiers** - 属性修饰器表
   - 存储效果对属性的具体修改
   - 支持加法、乘法等操作类型

### 支持的属性类型

- **通用属性**: Health, Energy, Speed
- **角色核心属性**: Intelligence, Perception, Charisma, Will, Constitution, Agility
- **角色派生属性**: LifeValue, MentalValue, MovementSpeed
- **飞船核心属性**: Thrust, Shield, Armor, Maneuverability, Sensors
- **飞船规格属性**: Mass, Length, Width, Height, CargoCapacity, FuelCapacity

## 🚀 快速开始

### 1. 安装依赖

确保项目中已安装以下 NuGet 包：

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.0.0" />
```

### 2. 初始化数据库

```csharp
using TO.Data.Scripts;

// 初始化数据库
string connectionString = "Data Source=player_attributes.db";
await InitializeDatabase.InitializeAsync(connectionString);
```

### 3. 基本使用

```csharp
using TO.Data.Services;
using TO.Data.Database;
using TO.Data.Repositories;
using Microsoft.EntityFrameworkCore;

// 创建数据库上下文
var options = new DbContextOptionsBuilder<AttributeDbContext>()
    .UseSqlite("Data Source=player_attributes.db")
    .Options;

using var context = new AttributeDbContext(options);
var repository = new AttributeSetRepository(context);
var mappingService = new AttributeSetMappingService();
var databaseService = new AttributeSetDatabaseService(repository, mappingService, context);
// 创建属性集
var attributeSet = new AttributeSet();

// 添加属性
attributeSet.SetAttribute(AttributeType.Health, new AttributeValue(AttributeType.Health, 100f, 0f, 200f));
attributeSet.SetAttribute(AttributeType.Energy, new AttributeValue(AttributeType.Energy, 50f, 0f, 100f));

// 创建技能
var fireball = new GameplayAbility("Fireball", "Fire damage spell")
{
    CooldownDuration = 5f,
    CastTime = 2f
};

// 设置消耗
fireball.SetCost(AttributeType.Energy, 25f);

// 保存到数据库
string playerId = "player_001";
string attributeSetName = "主属性集";
var savedId = await databaseService.SavePlayerAttributeSetAsync(
    playerId, attributeSetName, attributeSet, "玩家的主要属性集");

// 从数据库加载
var loadedAttributeSet = await databaseService.LoadPlayerAttributeSetAsync(playerId, attributeSetName);
```

## 📖 详细使用示例

查看 `Examples/DatabaseExample.cs` 文件，其中包含了完整的使用示例，包括：

- 数据库初始化
- 创建和保存属性集
- 加载和更新属性值
- 添加和管理效果
- 批量操作
- 数据库统计

运行示例：

```csharp
var example = new DatabaseExample();
await example.RunExampleAsync();
```

## 🔧 高级功能

### 属性效果系统

```csharp
// 创建临时效果
var healthBoostEffect = new GameplayEffect(
    "health_boost",
    "生命力增强",
    "临时增加生命值上限",
    EffectType.Temporary,
    EffectStackingType.Stack,
    new List<EffectTags> { EffectTags.Beneficial },
    new List<AttributeModifier>
    {
        new AttributeModifier(AttributeType.Health, ModifierOperationType.Add, 25f, SourceType.Item, "health_potion")
    },
    120f, // 持续2分钟
    false,
    3 // 最多堆叠3层
);

// 应用效果
attributeSet.ApplyEffect(healthBoostEffect);

// 保存到数据库
await databaseService.AddEffectToAttributeSetAsync(playerId, attributeSetName, healthBoostEffect);
```

### 批量操作

```csharp
// 批量保存多个玩家的属性集
var playerAttributeSets = new Dictionary<string, Dictionary<string, AttributeSet>>();
playerAttributeSets["player_001"] = new Dictionary<string, AttributeSet>
{
    ["主属性集"] = mainAttributeSet,
    ["备用属性集"] = backupAttributeSet
};

int savedCount = await databaseService.BatchSaveAttributeSetsAsync(playerAttributeSets);
```

### 数据库维护

```csharp
// 清理过期效果
int cleanedCount = await databaseService.CleanupExpiredEffectsAsync();

// 获取数据库统计
var stats = await databaseService.GetDatabaseStatsAsync();
foreach (var stat in stats)
{
    Console.WriteLine($"{stat.Key}: {stat.Value}");
}

// 重置数据库
await InitializeDatabase.ResetDatabaseAsync(connectionString);
```

## 🎯 最佳实践

1. **连接管理**: 使用依赖注入管理 `AttributeDbContext` 的生命周期
2. **异步操作**: 所有数据库操作都是异步的，避免阻塞主线程
3. **事务处理**: 复杂操作使用事务确保数据一致性
4. **错误处理**: 适当处理数据库异常和连接错误
5. **性能优化**: 使用批量操作处理大量数据
6. **数据验证**: 在保存前验证属性值的有效性

## 🔍 故障排除

### 常见问题

1. **数据库文件权限问题**
   - 确保应用程序有读写数据库文件的权限
   - 检查数据库文件路径是否正确

2. **实体映射错误**
   - 检查实体类的属性是否与数据库表结构匹配
   - 确保外键关系配置正确

3. **性能问题**
   - 使用 `Include()` 方法预加载相关数据
   - 考虑添加适当的数据库索引
   - 使用分页查询处理大量数据

### 调试技巧

```csharp
// 启用 EF Core 日志
var options = new DbContextOptionsBuilder<AttributeDbContext>()
    .UseSqlite(connectionString)
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine)
    .Options;
```

## 📝 许可证

本项目遵循 MIT 许可证。

## 🤝 贡献

欢迎提交 Issue 和 Pull Request 来改进这个项目。

---

**注意**: 这是一个为 Godot 游戏开发设计的数据库系统，专门用于管理游戏中的属性数据。在生产环境中使用前，请确保进行充分的测试。