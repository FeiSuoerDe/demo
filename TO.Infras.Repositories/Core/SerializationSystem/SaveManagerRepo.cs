using inFras.Bases;
using TO.Domains.Models.Repositories.Abstractions.Core.SerializationSystem;
using TO.Nodes.Abstractions.Nodes.Singletons;

namespace inFras.Core.SerializationSystem;

public class SaveManagerRepo: SingletonNodeRepo<ISaveManager>,ISaveManagerRepo
{
    protected override void Init()
    {
        base.Init();
        EmitReady();
    }
}