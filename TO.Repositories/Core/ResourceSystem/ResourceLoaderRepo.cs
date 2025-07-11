using System.Collections.Concurrent;
using Godot;
using GodotTask;
using TO.Commons.Enums.Infrastructure;
using TO.Repositories.Abstractions.Core.ResourceSystem;
using TO.Repositories.Bases;
using OS = Godot.OS;
using Array = Godot.Collections.Array;

namespace TO.Repositories.Core.ResourceSystem;

// 资源加载器仓库
public class ResourceLoaderRepo : BaseRepo, IResourceLoaderRepo
{
    private readonly CacheManager<string, Resource> _cache = new();
    private readonly SizeCalculator _sizeCalculator = new();
    private readonly ReferenceTracker _referenceTracker = new();
    private readonly MemoryMonitor _memoryMonitor = new();
    private readonly StatsCollector _statsCollector = new();
    private readonly ConcurrentQueue<string> _preloadQueue = new();
    private readonly SemaphoreSlim _preloadLock = new(1);
    
    public event Action<string, Resource?>? ResourceLoaded;
    public event Action<string>? ResourceUnloaded;
    
    public ResourceLoaderRepo()
    {
        _memoryMonitor.OnAutoReleaseTriggered += AutoReleaseResources;
        _referenceTracker.OnReferenceReleased += OnResourceReleased;
        _memoryMonitor.StartMonitoring();
    }
    

    public Resource? LoadResource(string path, CachePolicy policy = CachePolicy.Normal)
    {
        // 检查缓存
        if (_cache.TryGetValue(path, out var cachedResource))
        {
            _statsCollector.RecordCacheHit();
            _referenceTracker.Increase(path);
            return cachedResource.Resource;
        }

        _statsCollector.RecordCacheMiss();
        
        // 加载资源
        var resource = ResourceLoader.Load(path);
        if (resource == null)
            return null;
            
        // 计算大小并缓存
        var size = _sizeCalculator.Calculate(resource);
        var cacheNode = new CacheManager<string, Resource>.CacheNode
        {
            Resource = resource,
            Size = size,
            LastAccessed = DateTime.Now,
            Policy = policy
        };
        
        _cache.TryAdd(path, cacheNode);
        _referenceTracker.Increase(path);
        ResourceLoaded?.Invoke(path, resource);
        
        return resource;
    }

    public async Task<Resource?> LoadResourceAsync(string path, CachePolicy policy = CachePolicy.Normal, 
        Action<float>? progressCallback = null, float minLoadTime = 0.0f)
    {
        // 检查缓存
        if (_cache.TryGetValue(path, out var cachedResource))
        {
            _statsCollector.RecordCacheHit();
            _referenceTracker.Increase(path);
            progressCallback?.Invoke(1.0f); // 缓存命中，直接报告加载完成
            return cachedResource.Resource;
        }

        _statsCollector.RecordCacheMiss();
        
        // 开始异步加载
        var requestId = ResourceLoader.LoadThreadedRequest(path);
        var progress = new Array();
        var sceneTree = Engine.GetMainLoop()  as SceneTree;
        var tween = sceneTree?.CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        if (minLoadTime > 0)
        {
            if (progressCallback != null)
            {
                tween?.TweenMethod(Callable.From(progressCallback), 0f, 1f, minLoadTime);
            }

            if (tween != null) await tween.ToSignal(tween, Tween.SignalName.Finished);
            
            var status = ResourceLoader.LoadThreadedGetStatus(path, progress);
            
            if (status == ResourceLoader.ThreadLoadStatus.Failed)
                return null;
        }
        else
        {
            while (true)
            {
                var status = ResourceLoader.LoadThreadedGetStatus(path, progress);
                var currentProgress = progress[0].As<float>();
           

                if (minLoadTime > 0)
                {
                
                }
                if (status == ResourceLoader.ThreadLoadStatus.InProgress)
                {

                    await GDTask.DelayFrame(1);
                    continue;

                }

                if (status == ResourceLoader.ThreadLoadStatus.Failed)
                    return null;
                
                if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                    break;
            }
        }
        
        // 获取加载结果
        var resource = ResourceLoader.LoadThreadedGet(path);
        if (resource == null)
            return null;
            
        // 更新缓存和引用计数
        var size = _sizeCalculator.Calculate(resource);
        var cacheNode = new CacheManager<string, Resource>.CacheNode
        {
            Resource = resource,
            Size = size,
            LastAccessed = DateTime.Now,
            Policy = policy
        };
        
        _cache.TryAdd(path, cacheNode);
        _referenceTracker.Increase(path);
        ResourceLoaded?.Invoke(path, resource);
        
        return resource;
    }
    

    public void ClearCache(string path = "")
    {
        if (string.IsNullOrEmpty(path))
        {
            _cache.Clear();
        }
        else
        {
            _cache.TryRemove(path, out _);
        }
    }

    public void AddToPreloadQueue(string path)
    {
        _preloadQueue.Enqueue(path);
    }

    public async Task StartPreloading(Action<string, Resource>? onLoaded = null)
    {
        await _preloadLock.WaitAsync();
        try
        {
            while (_preloadQueue.TryDequeue(out var path))
            {
                var memoryInfo = OS.GetMemoryInfo();
                if (memoryInfo["available"].AsSingle() / memoryInfo["total"].AsSingle() < 0.3f)
                {
                    await GDTask.Delay(1000);
                    continue;
                }

                var resource = await LoadResourceAsync(path);
                if (resource != null)
                {
                    onLoaded?.Invoke(path, resource);
                }
            }
        }
        finally
        {
            _preloadLock.Release();
        }
    }

    public void AutoReleaseResources()
    {
        // 根据策略释放资源
        foreach (var item in _cache.GetAll())
        {
            if (item.Value.Policy == CachePolicy.Temporary)
            {
                _referenceTracker.Decrease(item.Key);
            }
            if(_referenceTracker.GetReferenceCount(item.Key) == 0)
            {
                OnResourceReleased(item.Key);
            }
        }
    }

    public void DecreaseReferenceCount(string path)
    {
        _referenceTracker.Decrease(path);
        if (_referenceTracker.GetReferenceCount(path) == 0)
        {
            OnResourceReleased(path);
        }
    }

    public void SetAutoReleaseThreshold(long bytes)
    {
        _memoryMonitor.SetThreshold(bytes);
    }

    public void SetLazyLoadInterval(float interval)
    {
        _memoryMonitor.SetInterval(interval);
    }
    
    private void OnResourceReleased(string path)
    {
        if (_cache.TryRemove(path, out var node))
        {
            ResourceUnloaded?.Invoke(path);
            node.Resource.Dispose();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _memoryMonitor.Dispose();
        }
        _memoryMonitor.OnAutoReleaseTriggered -= AutoReleaseResources;
        _referenceTracker.OnReferenceReleased -= OnResourceReleased;
        ClearCache();
    }
}