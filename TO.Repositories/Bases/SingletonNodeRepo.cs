using Godot;
using TO.Nodes.Abstractions.Bases;
using TO.Repositories.Abstractions.Bases;

namespace TO.Repositories.Bases;


public abstract class SingletonNodeRepo<TNode> : ISingletonNodeRepo<TNode>,IDisposable where TNode : INode
{
    private bool _disposed;
    public event Action? Ready;
    protected void EmitReady() => Ready?.Invoke();
    public event Action? TreeExiting;
    private void EmitTreeExiting() => TreeExiting?.Invoke();

    public TNode? Singleton { get; private set; }

    public bool Register(TNode singleton)
    {
        var result = Singleton is not null;
        Singleton = singleton;
        Singleton.TreeExiting += Unregister;
        
        Singleton.TreeExiting += EmitTreeExiting;
        Init();
        ConnectNodeEvents();
        return result;
    }

    public void Unregister()
    {
        if (Singleton == null)
        {
            GD.PrintErr("很奇怪，单例节点取消注册时已经为空了！");
            return;
        }

        Singleton.TreeExiting -= Unregister;
        
        Singleton.TreeExiting -= EmitTreeExiting;
        DisconnectNodeEvents();
        Singleton = default;
    }
    
    protected virtual  void Init()
    {
    }

    protected virtual void ConnectNodeEvents()
    {
    }

    protected virtual void DisconnectNodeEvents()
    {
    }

    public bool IsRegistered() => Singleton != null;
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this); // ✅ 抑制终结器
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        Unregister();
        _disposed = true;
    }
    
    ~SingletonNodeRepo() { // 终结器
        Dispose(false);
    }
}
