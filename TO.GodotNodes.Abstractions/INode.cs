using Autofac;

namespace TO.GodotNodes.Abstractions;

public interface INode
{
    event Action? Ready;
    event Action? TreeExiting; 
    ILifetimeScope? NodeScope { get; set; }
}