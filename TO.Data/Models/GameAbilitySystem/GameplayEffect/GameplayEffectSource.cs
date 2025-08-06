using System;
using System.Collections.Generic;
using Godot;
using TO.Commons.Enums.Game;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Models.GameAbilitySystem.GameplayEffect
{
    /// <summary>
    /// 效果来源
    /// </summary>
    public class GameplayEffectSource
    {
        /// <summary>
        /// 来源类型
        /// </summary>
        public SourceType SourceType { get; }

        /// <summary>
        /// 来源对象
        /// </summary>
        public object SourceObject { get; }

        /// <summary>
        /// 来源位置
        /// </summary>
        public Vector2 SourceLocation { get; }

        /// <summary>
        /// 来源的属性集
        /// </summary>
        public Dictionary<AttributeDefinition, float> Attributes { get; }

        public GameplayEffectSource(SourceType sourceType, object sourceObject, Vector2 sourceLocation, Dictionary<AttributeDefinition, float> attributes)
        {
            SourceType = sourceType;
            SourceObject = sourceObject;
            SourceLocation = sourceLocation;
            Attributes = attributes;
        }
    }
}
