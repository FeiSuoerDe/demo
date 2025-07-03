using Autofac;
using System.Reflection;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Contexts;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册所有命令处理程序和通知处理程序
        var handlersFromAssembly = Assembly.Load("TO.Apps.CommandHandles");
        var commandAssembly = Assembly.Load("TO.Apps.Commands");
        var appServicesAssembly = Assembly.Load("TO.Apps.Services");
        var configuration = MediatRConfigurationBuilder.Create(handlersFromAssembly,commandAssembly,appServicesAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(configuration);
    }
}
