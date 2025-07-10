using Autofac;
using Godot;
using TO.Nodes.Abstractions.Bases;
using TO.Repositories.Abstractions.Bases;

namespace TO.Repositories.Bases;

public abstract class NodeRepo<TNode> : INodeRepo<TNode>, IDisposable where TNode : class, INode
{
    private bool _disposed = false;
    public event Action? Ready;
    private void EmitReady() => Ready?.Invoke();
    public event Action? TreeExiting;
    private void EmitTreeExiting() => TreeExiting?.Invoke();
    
    public TNode? Node { get; protected set; }
    private ILifetimeScope? RepoScope { get; set; }
    
    protected void ConfigureNodeScope(ILifetimeScope lifetimeScope)
    {
        RepoScope = lifetimeScope;
    }

    protected void Register()
    {
        if (Node != null)
        {
            Node.TreeExiting += Unregister;

            Node.Ready += EmitReady;
            Node.TreeExiting += EmitTreeExiting;
        }

        Init();
        ConnectNodeEvents();
        
    }

    private void Unregister()
    {
        if (Node == null)
        {
            GD.PrintErr("很奇怪，单例节点取消注册时已经为空了！");
        }
        else
        {
            Node.TreeExiting -= Unregister;

            Node.Ready -= EmitReady;
            Node.TreeExiting -= EmitTreeExiting;
            Node = default;
        }

        RepoScope?.Dispose();
        DisconnectNodeEvents();
        
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
    
   
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this); // ✅ 抑制终结器
    }
    
    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                // 释放托管资源（如其他 IDisposable 对象）
            }
            Unregister();
            _disposed = true;
        }
    }
    
    ~NodeRepo() { // 终结器
        Dispose(false);
    }
}