# Task 1: 数据结构分析和映射关系确定

## 概述
本文档详细分析了当前数据库结构，确定了整数值与枚举名称的映射关系，并评估了迁移对现有功能的影响范围。

## 1. 当前数据库结构分析

### 1.1 AttributeValues表结构
当前数据库中存在一个统一的`AttributeValues`表，包含以下字段：
- `Id`: INTEGER PRIMARY KEY AUTOINCREMENT
- `AttributeSetId`: TEXT NOT NULL
- `AttributeType`: **INTEGER NOT NULL** (需要迁移的字段)
- `BaseValue`: REAL NOT NULL DEFAULT 0.0
- `CurrentValue`: REAL NOT NULL DEFAULT 0.0
- `MinValue`: REAL NOT NULL DEFAULT -999999.0
- `MaxValue`: REAL NOT NULL DEFAULT 999999.0
- `CreatedAt`: DATETIME DEFAULT CURRENT_TIMESTAMP
- `UpdatedAt`: DATETIME DEFAULT CURRENT_TIMESTAMP

### 1.2 现有数据分析
从数据库文件分析发现：
- 存在两个属性集：`player1-basic-attributes` 和 `player1-ship-attributes`
- AttributeType字段当前使用整数值：0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
- 数据完整性良好，所有记录都有有效的AttributeType值

## 2. 枚举映射关系确定

### 2.1 AttributeType枚举定义
基于`GameAbilitySystemEnums.cs`文件，AttributeType枚举包含以下值：

```csharp
public enum AttributeType
{
    // 通用属性 (0-2)
    Health = 0,
    Energy = 1,
    Speed = 2,
    
    // 角色核心属性 (3-8)
    Intelligence = 3,    // 智力
    Perception = 4,      // 感知
    Charisma = 5,        // 魅力
    Will = 6,            // 意志
    Constitution = 7,    // 体质
    Agility = 8,         // 敏捷
    
    // 角色派生属性 (9-11)
    LifeValue = 9,       // 生命值
    MentalValue = 10,    // 精神值
    MovementSpeed = 11,  // 移动速度
    
    // 飞船核心属性 (12-16)
    Thrust = 12,         // 推进力
    Shield = 13,         // 护盾
    Armor = 14,          // 装甲
    Maneuverability = 15,// 机动性
    Sensors = 16,        // 传感器
    
    // 飞船规格属性 (17-22)
    Mass = 17,           // 质量
    Length = 18,         // 长度
    Width = 19,          // 宽度
    Height = 20,         // 高度
    CargoCapacity = 21,  // 货舱容量
    FuelCapacity = 22,   // 燃料容量
    
    // 扩展属性 (23-25)
    CustomAttribute1 = 23,
    CustomAttribute2 = 24,
    CustomAttribute3 = 25
}
```

### 2.2 整数到枚举名称映射表

| 整数值 | 枚举名称 | 中文描述 | 属性分类 |
|--------|----------|----------|----------|
| 0 | Health | 生命值 | 通用属性 |
| 1 | Energy | 能量值 | 通用属性 |
| 2 | Speed | 速度 | 通用属性 |
| 3 | Intelligence | 智力 | 角色核心属性 |
| 4 | Perception | 感知 | 角色核心属性 |
| 5 | Charisma | 魅力 | 角色核心属性 |
| 6 | Will | 意志 | 角色核心属性 |
| 7 | Constitution | 体质 | 角色核心属性 |
| 8 | Agility | 敏捷 | 角色核心属性 |
| 9 | LifeValue | 生命值 | 角色派生属性 |
| 10 | MentalValue | 精神值 | 角色派生属性 |
| 11 | MovementSpeed | 移动速度 | 角色派生属性 |
| 12 | Thrust | 推进力 | 飞船核心属性 |
| 13 | Shield | 护盾 | 飞船核心属性 |
| 14 | Armor | 装甲 | 飞船核心属性 |
| 15 | Maneuverability | 机动性 | 飞船核心属性 |
| 16 | Sensors | 传感器 | 飞船核心属性 |
| 17 | Mass | 质量 | 飞船规格属性 |
| 18 | Length | 长度 | 飞船规格属性 |
| 19 | Width | 宽度 | 飞船规格属性 |
| 20 | Height | 高度 | 飞船规格属性 |
| 21 | CargoCapacity | 货舱容量 | 飞船规格属性 |
| 22 | FuelCapacity | 燃料容量 | 飞船规格属性 |
| 23 | CustomAttribute1 | 自定义属性1 | 扩展属性 |
| 24 | CustomAttribute2 | 自定义属性2 | 扩展属性 |
| 25 | CustomAttribute3 | 自定义属性3 | 扩展属性 |

## 3. 代码依赖关系分析

### 3.1 使用AttributeType字段的代码位置

#### 3.1.1 数据库相关代码
- `TO.Data.Database.AttributeDbContext.cs`: EF Core配置中定义了AttributeType字段
- `TO.Data.Models.GameAbilitySystem.GameplayAttribute.AttributeValue.cs`: 属性值模型类
- `TO.Data.Models.GameAbilitySystem.GameplayEffect.AttributeModifier.cs`: 属性修饰器模型类

#### 3.1.2 枚举定义
- `TO.Commons.Enums.Game.GameAbilitySystemEnums.cs`: AttributeType枚举定义

#### 3.1.3 潜在使用位置
- 仓储层：`TO.Repositories.Core.GameAbilitySystem.AttributeSetRepo.cs`
- 服务层：可能存在的属性管理服务
- 业务逻辑层：游戏逻辑中的属性计算和处理

### 3.2 数据库表分离需求
根据PRD要求，需要将当前的`AttributeValues`表分离为：
- `AttributeValues_BasicAttributes`: 存储角色基础属性和派生属性
- `AttributeValues_ShipAttributes`: 存储飞船相关属性

## 4. 迁移影响范围评估

### 4.1 数据库层面影响
- **高影响**: 需要修改表结构，将AttributeType从INTEGER改为TEXT
- **高影响**: 需要创建新的表结构（分离BasicAttributes和ShipAttributes）
- **中影响**: 需要迁移现有数据到新表结构
- **低影响**: 索引需要重新创建

### 4.2 代码层面影响
- **高影响**: Entity Framework模型需要更新
- **中影响**: 仓储层查询逻辑需要适配新表结构
- **中影响**: 服务层可能需要修改属性访问逻辑
- **低影响**: 枚举使用方式保持不变

### 4.3 功能层面影响
- **低影响**: 游戏逻辑层面的属性使用方式基本不变
- **中影响**: 属性查询和更新操作需要适配新的表结构
- **低影响**: 用户界面显示逻辑基本不受影响

## 5. 数据完整性验证

### 5.1 现有数据验证结果
- ✅ 所有AttributeType值都在有效范围内（0-11）
- ✅ 所有AttributeType值都有对应的枚举定义
- ✅ 数据库约束完整，无孤立记录
- ✅ 时间戳字段完整

### 5.2 映射关系验证
- ✅ 枚举值0-11都有明确的名称定义
- ✅ 中文描述完整
- ✅ 属性分类清晰

## 6. 详细测试计划

### 6.1 数据迁移测试
1. **备份测试**
   - 验证数据库备份功能
   - 确认备份文件完整性

2. **映射转换测试**
   - 验证每个整数值正确转换为对应枚举名称
   - 测试边界值处理
   - 验证特殊字符处理

3. **表分离测试**
   - 验证BasicAttributes表数据正确性
   - 验证ShipAttributes表数据正确性
   - 确认数据无丢失

### 6.2 功能回归测试
1. **属性读取测试**
   - 测试基础属性读取功能
   - 测试飞船属性读取功能
   - 验证属性值计算正确性

2. **属性更新测试**
   - 测试属性值更新功能
   - 测试批量更新操作
   - 验证事务处理

3. **查询性能测试**
   - 测试新索引性能
   - 对比迁移前后查询速度
   - 验证复杂查询正确性

### 6.3 集成测试
1. **EF Core集成测试**
   - 验证模型映射正确性
   - 测试LINQ查询功能
   - 验证变更跟踪

2. **业务逻辑集成测试**
   - 测试属性计算逻辑
   - 验证效果应用功能
   - 测试属性修饰器

## 7. 风险评估和缓解措施

### 7.1 主要风险
1. **数据丢失风险**: 迁移过程中可能出现数据丢失
   - 缓解措施：完整备份 + 分步迁移 + 验证检查

2. **性能下降风险**: TEXT类型可能影响查询性能
   - 缓解措施：优化索引 + 性能测试 + 监控

3. **兼容性风险**: 现有代码可能不兼容新结构
   - 缓解措施：全面测试 + 渐进式部署

### 7.2 回滚计划
1. 保留原始数据库备份
2. 准备回滚脚本
3. 制定紧急恢复流程

## 8. 下一步行动计划

1. **立即执行**:
   - 创建数据库备份
   - 准备迁移脚本

2. **短期计划**:
   - 更新Entity Framework模型
   - 修改数据库上下文配置

3. **中期计划**:
   - 执行数据迁移
   - 更新业务逻辑代码

4. **长期计划**:
   - 性能优化
   - 监控和维护

## 结论

通过详细分析，确认了以下关键信息：
- 当前数据库使用INTEGER存储AttributeType，值范围0-11
- 所有现有数据都有对应的枚举定义
- 迁移影响主要集中在数据库层和EF Core模型层
- 需要将单一AttributeValues表分离为两个专门的表
- 制定了完整的测试计划和风险缓解措施

该分析为后续的迁移实施提供了坚实的基础。