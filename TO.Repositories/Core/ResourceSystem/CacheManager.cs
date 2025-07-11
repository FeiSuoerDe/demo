using System.Collections.Concurrent;
using Godot;
using TO.Commons.Enums.Infrastructure;

namespace TO.Repositories.Core.ResourceSystem
{
    internal class CacheManager<TKey, TValue> where TValue : Resource
    {
        private readonly ConcurrentDictionary<TKey, CacheNode> _cache = new();
        private long _totalCacheSize;
        private readonly object _cacheLock = new();
        
        public long MaxCacheSize { get; set; } = 100 * 1024 * 1024; // 100MB默认
        
        public bool TryGetValue(TKey key, out CacheNode node) => _cache.TryGetValue(key, out node);
        
        public bool TryAdd(TKey key, CacheNode node) => _cache.TryAdd(key, node);
        
        public bool TryRemove(TKey key, out CacheNode node) => _cache.TryRemove(key, out node);
        
        public void Clear()
        {
            _cache.Clear();
            Interlocked.Exchange(ref _totalCacheSize, 0);
        }
        
        public long GetTotalSize() => Interlocked.Read(ref _totalCacheSize);
        
        public void UpdateSize(long delta)
        {
            Interlocked.Add(ref _totalCacheSize, delta);
        }
        
        public class CacheNode
        {
            public TValue Resource { get; set; }
            public long Size { get; set; }
            public DateTime LastAccessed { get; set; } = DateTime.Now;
            public CachePolicy Policy { get; set; }
        }

        public ConcurrentDictionary<TKey, CacheNode> GetAll()
        {
            return _cache;
        }
    }
}
