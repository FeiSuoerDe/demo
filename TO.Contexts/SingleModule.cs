using System.Reflection;
using Autofac;
using Domains.Core.AudioSystem;
using Infras.Writers.Abstractions;
using TO.Apps.Services.Core.UISystem;
using TO.Commons.Enums;
using TO.Domains.Services.Abstractions.Core.AudioSystem;
using Module = Autofac.Module;

namespace Contexts;

public class SingleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var appServicesAssembly = Assembly.Load("TO.Apps.Services");
        var absAppServicesAssembly = Assembly.Load("TO.Apps.Services.Abstractions");
        var servicesAssembly = Assembly.Load("TO.Domains.Services");
        var absServicesAssembly = Assembly.Load("TO.Domains.Services.Abstractions");
        var repoAssembly = Assembly.Load("TO.Infras.Repositories");
        var absRepoAssembly = Assembly.Load("TO.Domains.Models.Repositories.Abstractions");
       
        var appServiceTypes = appServicesAssembly.GetTypes()
            .Where(t => !t.Name.StartsWith("Node")).ToList();
        var serviceTypes = servicesAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Service")
                        && !t.Name.StartsWith("Node")).ToList();
        var repoTypes = repoAssembly.GetTypes()
            .Where(t => !t.Name.StartsWith("Node")
                        && !t.Name.StartsWith("Base")
                        && !t.Name.StartsWith("Singleton")).ToList();
        
        var absServicesInterfaces = absServicesAssembly.GetTypes()
            .ToDictionary(t => t.Name, t => t);
        var absAppServiceInterfaces = absAppServicesAssembly.GetTypes()
            .ToDictionary(t => t.Name, t => t);
        var absRepoInterfaces = absRepoAssembly.GetTypes()
            .ToDictionary(t => t.Name, t => t);
        
        
        RegisterTypes(appServiceTypes, absAppServiceInterfaces, builder);
        RegisterTypes(serviceTypes, absServicesInterfaces, builder);
        RegisterTypes(repoTypes, absRepoInterfaces, builder);

        builder.RegisterType<UIManagerAppService>().AsSelf().SingleInstance();
        builder.RegisterType<AudioManagerService>().As<IAudioManagerService>()
            .As<IDataAccess>().WithMetadata("Key", LoadType.Auto).SingleInstance();
    }
    
    private void RegisterTypes(IEnumerable<Type> implTypes, IDictionary<string, Type> interfaceDict,ContainerBuilder builder)
    {
        foreach (var implType in implTypes)
        {
            var interfaceName = "I" + implType.Name;
            if (interfaceDict.TryGetValue(interfaceName, out var interfaceType))
            {
                
                builder.RegisterType(implType).As(interfaceType).SingleInstance();
            }
            
        }
    }
}