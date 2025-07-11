using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 动画上下文，包含执行动画所需的所有信息
/// </summary>
public class AnimationContext : IDisposable
{
    private bool  _disposed;
    /// <summary>
    /// 控制节点
    /// </summary>
    public Control Control { get; }
    
    /// <summary>
    /// 动画参数
    /// </summary>
    public BaseAnimationParameters Parameters { get; }
    
    /// <summary>
    /// 补间动画对象
    /// </summary>
    public Tween? Tween { get; private set; }
    
    /// <summary>
    /// 取消令牌
    /// </summary>
    public CancellationToken CancellationToken { get; }

    public AnimationContext(Control control, BaseAnimationParameters parameters, CancellationToken cancellationToken)
    {
        Control = control;
        Parameters = parameters;
        CancellationToken = cancellationToken;
        
        // 创建补间动画对象
        Tween = control.CreateTween();
        Tween.SetEase(parameters.Ease)
            .SetTrans(parameters.Trans);
    }

    /// <summary>
    /// 清理上下文资源
    /// </summary>
    public void Cleanup()
    {
        if (Tween != null && !Tween.IsValid()) return;
        Tween?.Kill(); 
        Tween?.Dispose();
        Tween = null;
    }

    public void Dispose()
    {
        if  (_disposed)
            return;
        Cleanup();
        GC.SuppressFinalize(this);
        _disposed = true;
    }
    ~AnimationContext()
    {
        Dispose();
    }
}
