using Autofac;

namespace TO.Nodes.Abstractions.Bases;

public interface INode
{
    event Action? Ready;
    event Action? TreeExiting; 
    ILifetimeScope? NodeScope { get; set; }
}