namespace TO.Commons;

public class LazySingleton<T> : IDisposable where T : new()
{
    private bool _disposed;
    protected static T? instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();

                return instance;
            }

            return instance;
        }
    }
    
    protected virtual void UnSubscriber(){}
    
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) {
            
        }
            
        _disposed = true;
    }
    
    ~LazySingleton() { // 终结器
        Dispose(false);
    }
}
