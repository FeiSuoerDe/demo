using System.Reflection;
using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace TO.Contexts;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册所有命令处理程序和通知处理程序
        var handlersFromAssembly = Assembly.Load("TO.CommandHandlers");
        var commandAssembly = Assembly.Load("TO.Commands");
        var servicesAssembly = Assembly.Load("TO.Services");
        var configuration = MediatRConfigurationBuilder.Create(handlersFromAssembly,commandAssembly,servicesAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(configuration);
    }
}
