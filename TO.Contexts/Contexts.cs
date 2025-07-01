using System.Reflection;
using Autofac;
using TO.Apps.Services.Abstractions.Core.SerializationSystem;
using TO.Apps.Services.Core.UISystem;
using TO.Commons;
using TO.Domains.Models.Repositories.Abstractions.Core.AudioSystem;
using TO.Domains.Models.Repositories.Abstractions.Nodes;
using TO.Events.Contexts;
using TO.GodotNodes.Abstractions;

namespace Contexts;

public class Contexts : LazySingleton<Contexts>
{
    private IContainer Container { get; }
    
    private NodeRegister? _nodeRegister;
    
    public bool RegisterNode<T>(T singleton) where T : INode
    {
        return _nodeRegister != null && _nodeRegister.Register(singleton);
    }
    public Contexts()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<NodeRegister>().SingleInstance();
        builder.RegisterModule<SingleModule>();
        builder.RegisterModule<NodeModule>();
        builder.RegisterModule<EventBusModule>();
        builder.RegisterModule<MediatorModule>();
        
        Container = builder.Build();
        
        _nodeRegister = Container.Resolve<NodeRegister>();
        Container.Resolve<IAudioManagerRepo>();
        Container.Resolve<ISaveManagerAppService>();
        Container.Resolve<UIManagerAppService>();
        ContextEvents.RegisterNodeRepo += OnRegisterNodeRepo;
    }

    private void OnRegisterNodeRepo(object o, Action<ILifetimeScope>? action)
    {
        var repoType = o.GetType();
        var scope = Container.BeginLifetimeScope(repoType,builder =>
        {
            builder.RegisterInstance(o).AsImplementedInterfaces().SingleInstance();
            
        });
        var commandType = GetCorrespondingAppServiceType(repoType);
        if (commandType != null) scope.Resolve(commandType);
        
        var serviceType = GetCorrespondingServiceType(repoType);
        if (serviceType != null) scope.Resolve(serviceType);
        
        action?.Invoke(scope);
    }
    
    // 根据 Repo 类型推导出对应的 Service 类型
    private Type? GetCorrespondingAppServiceType(Type repoType)
    {
        var repoName = repoType.Name;

        if (!repoName.EndsWith("Repo")) return null;
        var serviceName = repoName.Replace("Repo", "Service");
        var assembly = Assembly.Load("TO.Apps.Services");
        
        var appServiceType = assembly.GetTypes().FirstOrDefault(t => 
            t.Name == serviceName
        );
        return appServiceType;
    }
    
    private Type? GetCorrespondingServiceType(Type repoType)
    {
        
        var repoName = repoType.Name;

        if (!repoName.EndsWith("Repo")) return null;
        var serviceName = repoName.Replace("Repo", "Service");
        var assembly = Assembly.Load("TO.Domains.Services");
            
        var serviceType = assembly.GetTypes().FirstOrDefault(t => 
            t.Name == serviceName
        );
        return serviceType;
    }

    protected override void UnSubscriber()
    {
        base.UnSubscriber();
        ContextEvents.RegisterNodeRepo -= OnRegisterNodeRepo;
    }
    
}