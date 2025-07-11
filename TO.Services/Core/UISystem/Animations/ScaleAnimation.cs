using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 缩放动画实现
/// </summary>
public class ScaleAnimation : AnimationBase
{
    /// <summary>
    /// 执行缩放动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="fromScale">起始缩放</param>
    /// <param name="toScale">目标缩放</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task ScaleAsync(Control control, Vector2 fromScale, Vector2 toScale,
        float duration = 0.3f, Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        var parameters = new ScaleAnimationParameters
        {
            Duration = duration,
            Ease = ease,
            Trans = trans,
            FromScale = fromScale,
            ToScale = toScale
        };

        var animation = new ScaleAnimation();
        return animation.ExecuteAsync(control, parameters, cancellationToken);
    }

    /// <summary>
    /// 执行放大动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task ScaleInAsync(Control control, float duration = 0.3f,
        Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        return ScaleAsync(
            control,
            new Vector2(0, 0),
            new Vector2(1, 1),
            duration,
            ease,
            trans,
            cancellationToken
        );
    }

    /// <summary>
    /// 执行缩小动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task ScaleOutAsync(Control control, float duration = 0.3f,
        Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        return ScaleAsync(
            control,
            new Vector2(1, 1),
            new Vector2(0, 0),
            duration,
            ease,
            trans,
            cancellationToken
        );
    }

    /// <summary>
    /// 执行缩放动画的核心逻辑
    /// </summary>
    protected override async Task ExecuteAnimationCore(AnimationContext? context, CancellationToken cancellationToken)
    {
        
        if (context == null) return;

        var parameters = context.Parameters as ScaleAnimationParameters;
        var control = context.Control;
        var tween = context.Tween;

        // 检查参数和控制节点有效性
        if (parameters == null || tween == null) return;

        // 设置初始缩放
        control.Scale = parameters.FromScale;

        // 创建缩放动画
        tween.TweenProperty(
                control,
                "scale",
                parameters.ToScale,
                parameters.Duration
            ).SetEase(parameters.Ease)
            .SetTrans(parameters.Trans);


        // 使用TaskCompletionSource<object>避免冗余bool值
        var animationCompletedTcs = new TaskCompletionSource<object?>();
    
        // 注册动画完成回调
        tween.Finished += () => animationCompletedTcs.TrySetResult(null);
    
        // 用CancellationToken注册取消回调（替代Task.Delay）
        await using var ctsRegistration = context.CancellationToken.Register(() => 
        {
            animationCompletedTcs.TrySetCanceled(); // 标记任务为取消状态
        });
    
        try
        {
            // 等待动画完成或取消
            await animationCompletedTcs.Task;
        }
        catch (TaskCanceledException)
        {
            // 取消时抛出统一异常
            context.CancellationToken.ThrowIfCancellationRequested();
        }

    }
}
