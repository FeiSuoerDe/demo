using Godot;

namespace inFras.Core.ResourceSystem
{
    public interface IResourceLoader
    {
        Resource? Load<T>(string path) where T : Resource;
    }
}
