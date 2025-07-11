namespace TO.Repositories.Core.ResourceSystem
{
    internal class StatsCollector
    {
        private int _cacheHits;
        private int _cacheMisses;
        private int _loadingCount;
        private int _autoReleaseCount;
        
        public void RecordCacheHit() => Interlocked.Increment(ref _cacheHits);
        public void RecordCacheMiss() => Interlocked.Increment(ref _cacheMisses);
        public void RecordLoading() => Interlocked.Increment(ref _loadingCount);
        public void RecordAutoRelease() => Interlocked.Increment(ref _autoReleaseCount);
        
        public float GetHitRate()
        {
            var hits = Interlocked.CompareExchange(ref _cacheHits, 0, 0);
            var misses = Interlocked.CompareExchange(ref _cacheMisses, 0, 0);
            var total = hits + misses;
            return total > 0 ? hits * 100f / total : 0;
        }

        public string GetStatsSnapshot()
        {
            return $@"资源统计快照:
- 缓存命中率: {GetHitRate():F1}%
- 缓存命中: {Interlocked.CompareExchange(ref _cacheHits, 0, 0)}
- 缓存未命中: {Interlocked.CompareExchange(ref _cacheMisses, 0, 0)}
- 正在加载: {Interlocked.CompareExchange(ref _loadingCount, 0, 0)}
- 自动释放次数: {Interlocked.CompareExchange(ref _autoReleaseCount, 0, 0)}";
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _cacheHits, 0);
            Interlocked.Exchange(ref _cacheMisses, 0);
            Interlocked.Exchange(ref _loadingCount, 0);
            Interlocked.Exchange(ref _autoReleaseCount, 0);
        }
    }
}
