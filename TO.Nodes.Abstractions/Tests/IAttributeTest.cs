using TO.Nodes.Abstractions.Bases;

namespace TO.Nodes.Abstractions.Tests;

public interface IAttributeTest : INode
{

    Guid AttributeSetId { get; set; }
}