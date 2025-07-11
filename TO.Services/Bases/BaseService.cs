namespace TO.Services.Bases;

public abstract class BaseService : IDisposable
{
    private bool _disposed; // 释放标记
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    // 实现 IDisposable.Dispose()
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // 阻止终结器调用[1,2,6](@ref)
    }

    // 受保护的虚方法，支持派生类扩展
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        UnSubscriber();
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();

        _disposed = true;
    }

    protected virtual void UnSubscriber(){}

    // 终结器（析构函数），用于未显式调用 Dispose 时的补救
    ~BaseService()
    {
        Dispose(false); // 仅释放非托管资源[1,4,6](@ref)
    }
}