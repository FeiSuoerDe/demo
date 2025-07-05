using Autofac;
using TO.Commons;
using TO.Domains.Models.Repositories.Abstractions.Bases;
using TO.GodotNodes.Abstractions;

namespace Contexts;

public class NodeContexts : LazySingleton<NodeContexts>
{
    private IContainer Container { get; }
    
    public NodeContexts()
    {
        var builder = new ContainerBuilder();
        Container = builder.Build();
    }
    
    public ILifetimeScope RegisterNode<TNode, TRepo>(TNode scene)
        where TNode : class, INode
        where TRepo : class, INodeRepo<TNode>
    {
        var scope = Container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(scene).As<INode>().As<TNode>().SingleInstance();
            builder.RegisterType<TRepo>().SingleInstance();
        });
        scope.Resolve<TRepo>();
        return scope;
    }
    
    
}
