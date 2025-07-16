using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace TO.Contexts;

public class SingleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var servicesAssembly = Assembly.Load("TO.Services");
        var absServicesAssembly = Assembly.Load("TO.Services.Abstractions");
        var repoAssembly = Assembly.Load("TO.Repositories");
        var absRepoAssembly = Assembly.Load("TO.Repositories.Abstractions");
        
        var serviceTypes = servicesAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Service")
                        && !t.Name.StartsWith("Node")).ToList();
        var repoTypes = repoAssembly.GetTypes()
            .Where(t => !t.Name.StartsWith("Node")
                        && !t.Name.StartsWith("Base")
                        && !t.Name.StartsWith("Singleton")).ToList();
        
        var absServicesInterfaces = absServicesAssembly.GetTypes()
            .ToDictionary(t => t.Name, t => t);
       
        var absRepoInterfaces = absRepoAssembly.GetTypes()
            .ToDictionary(t => t.Name, t => t);
        
        RegisterTypes(serviceTypes, absServicesInterfaces, builder);
        RegisterTypes(repoTypes, absRepoInterfaces, builder);
        ;
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