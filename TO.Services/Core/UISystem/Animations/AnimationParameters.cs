using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 淡入淡出动画参数
/// </summary>
public record FadeAnimationParameters : BaseAnimationParameters
{
    /// <summary>
    /// 起始透明度
    /// </summary>
    public float FromAlpha { get; init; }
    
    /// <summary>
    /// 目标透明度
    /// </summary>
    public float ToAlpha { get; init; }

    public override bool Validate()
    {
        return base.Validate() && 
               FromAlpha is >= 0 and <= 1 && 
               ToAlpha is >= 0 and <= 1;
    }
}

/// <summary>
/// 缩放动画参数
/// </summary>
public record ScaleAnimationParameters : BaseAnimationParameters
{
    /// <summary>
    /// 起始缩放
    /// </summary>
    public Vector2 FromScale { get; init; }
    
    /// <summary>
    /// 目标缩放
    /// </summary>
    public Vector2 ToScale { get; init; }

    public override bool Validate()
    {
        return base.Validate() && 
               FromScale is { X: > 0, Y: > 0 } && 
               ToScale is { X: > 0, Y: > 0 };
    }
}

/// <summary>
/// 移动动画参数
/// </summary>
public record MoveAnimationParameters : BaseAnimationParameters
{
    /// <summary>
    /// 起始位置
    /// </summary>
    public Vector2 FromPosition { get; init; }
    
    /// <summary>
    /// 目标位置
    /// </summary>
    public Vector2 ToPosition { get; init; }

    public override bool Validate()
    {
        return base.Validate();
    }
}
