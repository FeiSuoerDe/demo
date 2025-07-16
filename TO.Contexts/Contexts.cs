using Autofac;
using TO.Commons;
using TO.Nodes.Abstractions.Bases;
using TO.Repositories.Bases;
using TO.Services.Abstractions.Core.SequenceSystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Contexts;

public class Contexts : LazySingleton<Contexts>
{
    private IContainer Container { get; init; }
    
    private NodeRegister? _nodeRegister;
    
    public bool RegisterSingleNode<T>(T singleton) where T : INode
    {
        return _nodeRegister != null && _nodeRegister.Register(singleton);
    }
    public Contexts()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<NodeRegister>().SingleInstance();
        builder.RegisterModule<SingleModule>();
        builder.RegisterModule<MediatorModule>();
        
        Container = builder.Build();
        
        _nodeRegister = Container.Resolve<NodeRegister>();
        Container.Resolve<IUIManagerService>();
        Container.Resolve<ISequenceManagerService>();
    }
    
    public ILifetimeScope RegisterNode<TNode, TRepo, TService>(TNode scene)
        where TNode : class, INode
        where TRepo : NodeRepo<TNode>
        where TService : BaseService
    {
        var scope = Container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(scene).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TRepo>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        });
        scope.Resolve<TRepo>();
        scope.Resolve<TService>();
        return scope;
    }
    
    public ILifetimeScope RegisterNode<TNode, TService>(TNode scene)
        where TNode : class, INode
        where TService : BaseService
    {
        var scope = Container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(scene).AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterType<TService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        });
        scope.Resolve<TService>();
        return scope;
    }
    
}