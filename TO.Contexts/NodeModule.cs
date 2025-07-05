using System.Reflection;
using Autofac;
using Module = Autofac.Module;


namespace Contexts;

public class NodeModule: Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var appServicesAssembly = Assembly.Load("TO.Apps.Services");
        var repoAssembly = Assembly.Load("TO.Infras.Repositories");
        var servicesTypes = appServicesAssembly.GetTypes()
            .Where(t => t.Name.StartsWith("Node"));

        foreach (var commandType in servicesTypes)
        {
            var repoTypeName = commandType.Name.Replace("Service", "Repo");
            var repoType = repoAssembly.GetTypes().FirstOrDefault(t => 
                t.Name == repoTypeName
            );
            if (repoType != null)
            {
                builder.RegisterType(commandType)
                    .AsSelf()
                    .InstancePerMatchingLifetimeScope(repoType);
            }
        }
    }
}