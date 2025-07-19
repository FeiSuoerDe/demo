using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.Tests;

public interface IAttributeTest : INode
{
    event Action? OnHurt;
    event Action? OnHeal;
    
    Guid AttributeSetId { get; set; }
}