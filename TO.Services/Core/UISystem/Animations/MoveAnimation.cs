using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 移动动画实现
/// </summary>
public class MoveAnimation : AnimationBase
{
    /// <summary>
    /// 执行移动动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="toPosition">目标位置</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task MoveAsync(Control control, Vector2 fromPosition, Vector2 toPosition, 
        float duration = 0.3f, Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        var parameters = new MoveAnimationParameters
        {
            Duration = duration,
            Ease = ease,
            Trans = trans,
            FromPosition = fromPosition,
            ToPosition = toPosition
        };
        
        var animation = new MoveAnimation();
        return animation.ExecuteAsync(control, parameters, cancellationToken);
    }
    
    /// <summary>
    /// 执行移入动画（从指定位置移动到当前位置）
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task MoveInAsync(Control control, Vector2 fromPosition, 
        float duration = 0.3f, Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        return MoveAsync(
            control,
            fromPosition,
            control.Position,
            duration,
            ease,
            trans,
            cancellationToken
        );
    }
    
    /// <summary>
    /// 执行移出动画（从当前位置移动到指定位置）
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="toPosition">目标位置</param>
    /// <param name="duration">持续时间</param>
    /// <param name="ease">缓动类型</param>
    /// <param name="trans">过渡类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public static Task MoveOutAsync(Control control, Vector2 toPosition, 
        float duration = 0.3f, Tween.EaseType ease = Tween.EaseType.Out, 
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        CancellationToken cancellationToken = default)
    {
        return MoveAsync(
            control,
            control.Position,
            toPosition,
            duration,
            ease,
            trans,
            cancellationToken
        );
    }

    /// <summary>
    /// 执行移动动画的核心逻辑
    /// </summary>
    protected override async Task ExecuteAnimationCore(AnimationContext? context, CancellationToken cancellationToken)
    {
        if (context != null)
        {
            var parameters = (MoveAnimationParameters)context.Parameters;
            var control = context.Control;
            var tween = context.Tween;
        
            // 设置初始位置
            control.Position = parameters.FromPosition;
        
            // 创建位置动画
            tween?.TweenProperty(
                    control,
                    "position",
                    parameters.ToPosition,
                    parameters.Duration
                ).SetEase(parameters.Ease)
                .SetTrans(parameters.Trans);
        
            // 等待动画完成或取消
            // if (tween != null) await tween.ToSignal(tween, Tween.SignalName.Finished);
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
