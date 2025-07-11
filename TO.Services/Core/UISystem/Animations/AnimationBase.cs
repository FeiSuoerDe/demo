using Godot;

namespace TO.Services.Core.UISystem.Animations;

/// <summary>
/// 动画基类，提供基础的动画执行框架
/// </summary>
public abstract class AnimationBase : IDisposable
{
    
    private bool  _disposed;
    /// <summary>
    /// 执行动画
    /// </summary>
    /// <param name="control">控制节点</param>
    /// <param name="parameters">动画参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    public async Task ExecuteAsync(Control control, BaseAnimationParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(control);

        ArgumentNullException.ThrowIfNull(parameters);
        if (!parameters.Validate())
            throw new ArgumentException("动画参数无效", nameof(parameters));

        var context = new AnimationContext(control, parameters, cancellationToken);
        
        try
        {
            await ExecuteAnimationCore(context, cancellationToken);
            context.Dispose();
            Dispose();
        }
        catch (OperationCanceledException)
        {
            // 动画被取消，清理资源
            context.Dispose();
            Dispose();
            throw;
        }
        catch (Exception)
        {
            // 发生其他错误，清理资源
            context.Dispose();
            Dispose();
            throw;
        }
    }

    /// <summary>
    /// 执行动画的核心逻辑，由具体的动画类实现
    /// </summary>
    /// <param name="context">动画上下文</param>
    /// <param name="cancellationToken"></param>
    /// <returns>异步任务</returns>
    protected abstract Task ExecuteAnimationCore(AnimationContext? context, CancellationToken cancellationToken);

   
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~AnimationBase()
    {
        Dispose(false);
    }
}
