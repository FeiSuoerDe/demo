using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 基础动画参数实现
/// </summary>
public record BaseAnimationParameters 
{
   
    public float Duration { get; init; } = 0.3f;
    public Tween.EaseType Ease { get; init; } = Tween.EaseType.Out;
    public Tween.TransitionType Trans { get; init; } = Tween.TransitionType.Cubic;

    public virtual bool Validate()
    {
        return Duration > 0;
    }
    
}
