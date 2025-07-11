using TO.Nodes.Abstractions.Singletons;
using TO.Repositories.Abstractions.Core.SerializationSystem;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.SerializationSystem;

public class SaveManagerRepo: SingletonNodeRepo<ISaveManager>,ISaveManagerRepo
{
    protected override void Init()
    {
        base.Init();
        EmitReady();
    }
}