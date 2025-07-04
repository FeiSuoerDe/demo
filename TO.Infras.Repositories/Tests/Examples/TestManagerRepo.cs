using Godot;
using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Test.Examples;
using TO.Nodes.Abstractions.Tests.Examples;

namespace inFras.Tests.Examples;

public class TestManagerRepo : SingletonNodeRepo<ITestManager>, ITestManagerRepo
{
    protected override void Init()
    {
        base.Init();
        GD.Print("TestManagerRepo Init");
    }
}