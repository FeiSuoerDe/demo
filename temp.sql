DROP TABLE IF EXISTS AttributeEffects;
CREATE TABLE AttributeEffects (
    Id TEXT,
    Name TEXT NOT NULL,
    Description TEXT,
    EffectType TEXT NOT NULL, -- 效果类型: Instant, Duration, Infinite
    StackingType TEXT NOT NULL, -- 堆叠类型: NoStack, Stack, Replace
    Tags TEXT, -- 标签: 用逗号分隔的标签列表
    DurationSeconds REAL, -- 持续时间（秒）, 用于Duration类型
    IsInfinite BOOLEAN, -- 是否无限持续, 用于Infinite类型
    MaxStacks INTEGER, -- 最大堆叠层数
    IsPassive BOOLEAN, -- 是否被动效果
    Priority INTEGER, -- 优先级
    IsPeriodic BOOLEAN DEFAULT 0, -- 是否周期性效果
    IntervalSeconds REAL DEFAULT 1.0, -- 周期间隔（秒）, 如果IsPeriodic为true
    PRIMARY KEY (Id)
);
INSERT INTO AttributeEffects SELECT * FROM AttributeEffects_old;
DROP TABLE AttributeEffects_old;