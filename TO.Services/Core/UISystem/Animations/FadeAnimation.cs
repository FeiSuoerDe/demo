using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 淡入淡出动画实现
/// </summary>
public class FadeAnimation : AnimationBase
{
    /// <summary>
    /// 执行淡入动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken"></param>
    /// <returns>异步任务</returns>
    public static Task FadeInAsync(Control control, float duration = 0.3f, 
        Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        var parameters = new FadeAnimationParameters
        {
            Duration = duration,
            Ease = ease,
            Trans = trans,
            FromAlpha = 0,
            ToAlpha = 1
        };
        
        var animation = new FadeAnimation();
        return animation.ExecuteAsync(control, parameters, cancellationToken);
    }

    /// <summary>
    /// 执行淡出动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken"></param>
    /// <returns>异步任务</returns>
    public static Task FadeOutAsync(Control control, float duration = 0.3f, 
        Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        var parameters = new FadeAnimationParameters
        {
            Duration = duration,
            Ease = ease,
            Trans = trans,
            FromAlpha = 1,
            ToAlpha = 0
        };
        
        var animation = new FadeAnimation();
        return animation.ExecuteAsync(control, parameters, cancellationToken);
    }

    /// <summary>
    /// 执行自定义淡入淡出动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="fromAlpha">起始透明度</param>
    /// <param name="toAlpha">目标透明度</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken"></param>
    /// <returns>异步任务</returns>
    public static Task FadeAsync(Control control, float fromAlpha, float toAlpha,
        float duration = 0.3f, Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        var parameters = new FadeAnimationParameters
        {
            Duration = duration,
            Ease = ease,
            Trans = trans,
            FromAlpha = fromAlpha,
            ToAlpha = toAlpha
        };
        
        var animation = new FadeAnimation();
        return animation.ExecuteAsync(control, parameters, cancellationToken);
    }

    /// <summary>
    /// 执行淡入淡出动画的核心逻辑
    /// </summary>
    protected override async Task ExecuteAnimationCore(AnimationContext? context, CancellationToken cancellationToken)
    {
        if (context != null)
        {
            var parameters = (FadeAnimationParameters)context.Parameters;
            var control = context.Control;
            var tween = context.Tween;
        
            // 设置初始透明度
            control.Modulate = new Color(
                control.Modulate.R,
                control.Modulate.G,
                control.Modulate.B,
                parameters.FromAlpha
            );
        
            // 创建透明度动画
            tween?.TweenProperty(
                    control,
                    "modulate:a",
                    parameters.ToAlpha,
                    parameters.Duration
                ).SetEase(parameters.Ease)
                .SetTrans(parameters.Trans);
        
            // 等待动画完成或取消
            if (tween != null)
            {
                // 创建TaskCompletionSource来转换Tween信号
                var animationCompletedTcs = new TaskCompletionSource<bool>();
            
                // 监听动画完成
                tween.Finished += () => animationCompletedTcs.TrySetResult(true);
            
                // 创建可取消的Task
                var cancellationTask = Task.Delay(-1, cancellationToken)
                    .ContinueWith(t => {
                        // 取消时停止Tween
                        tween.Stop();
                        return t.IsCanceled;
                    }, TaskContinuationOptions.ExecuteSynchronously);

                // 等待任意一个Task先完成
                await Task.WhenAny(animationCompletedTcs.Task, cancellationTask);
            
                // 确保取消时资源释放
                if (cancellationToken.IsCancellationRequested)
                {
                    tween.Stop();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
