using System.Collections.Concurrent;

namespace TO.Repositories.Core.ResourceSystem
{
    internal class ReferenceTracker
    {
        private readonly ConcurrentDictionary<string, int> _referenceCounts = new();
        private readonly object _refCountLock = new();

        public event Action<string>? OnReferenceReleased;

        public int GetReferenceCount(string path)
        {
            lock (_refCountLock)
            {
                return _referenceCounts.GetValueOrDefault(path, 0);
            }
        }

        public void Increase(string path)
        {
            lock (_refCountLock)
            {
                _referenceCounts.AddOrUpdate(path, 
                    _ => 1, 
                    (_, count) => count + 1);
            }
        }

        public void Decrease(string path)
        {
            lock (_refCountLock)
            {
                if (_referenceCounts.TryGetValue(path, out var count) && count > 0)
                {
                    _referenceCounts[path] = count - 1;
                    if (count == 1)
                    {
                        OnReferenceReleased?.Invoke(path);
                    }
                }
            }
        }

        public void Remove(string path)
        {
            lock (_refCountLock)
            {
                _referenceCounts.TryRemove(path, out _);
            }
        }
    }
}
