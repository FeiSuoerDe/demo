using Godot;
using TO.Commons.Enums.Infrastructure;

namespace TO.Repositories.Abstractions.Core.ResourceSystem;

public interface IResourceLoaderRepo
{
    event Action<string, Resource?> ResourceLoaded;
    event Action<string> ResourceUnloaded;
    Resource? LoadResource(string path, CachePolicy policy = CachePolicy.Normal);
    Task<Resource?> LoadResourceAsync(string path, CachePolicy policy = CachePolicy.Normal,
        Action<float>? progressCallback = null, float minLoadTime = 1.0f);

    void DecreaseReferenceCount(string path);
    void ClearCache(string path = "");
    void AutoReleaseResources();
    void SetAutoReleaseThreshold(long bytes);
    void SetLazyLoadInterval(float interval);
    void Dispose();
}