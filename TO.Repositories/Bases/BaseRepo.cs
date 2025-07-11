namespace TO.Repositories.Bases;

public abstract class BaseRepo: IDisposable {
    protected bool _disposed = false;
    protected CancellationTokenSource  _cancellationTokenSource = new();
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this); // ✅ 抑制终结器
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) {
            // 释放托管资源（如对象、数组）
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        // 释放非托管资源（如文件句柄、数据库连接）
        _disposed = true;
    }
    
    ~BaseRepo() { // 终结器
        Dispose(false);
    }
}