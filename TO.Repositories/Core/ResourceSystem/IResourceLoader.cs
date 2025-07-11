using Godot;

namespace TO.Repositories.Core.ResourceSystem
{
    public interface IResourceLoader
    {
        Resource? Load<T>(string path) where T : Resource;
    }
}
