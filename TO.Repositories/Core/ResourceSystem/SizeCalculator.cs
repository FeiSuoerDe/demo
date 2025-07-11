using System.Collections.Concurrent;
using Godot;

namespace TO.Repositories.Core.ResourceSystem
{
    internal class SizeCalculator
    {
        private readonly ConcurrentDictionary<string, long> _sizeCache = new();
        private readonly object _sizeLock = new();

        public long Calculate(Resource resource)
        {
            if (resource == null) return 0;
            
            // 检查缓存
            if (!string.IsNullOrEmpty(resource.ResourcePath) && 
                _sizeCache.TryGetValue(resource.ResourcePath, out var cachedSize))
                return cachedSize;
            
            long size = 0;
            
            // 根据资源类型精确计算
            switch (resource)
            {
                case Texture2D texture:
                    // 纹理大小 = 宽 * 高 * 通道数(4) * mipmap级别
                    size = texture.GetWidth() * texture.GetHeight() * 4;
                    break;
                    
                case AudioStreamWav wav:
                    // WAV大小 = 采样率 * 位深/8 * 通道数 * 时长(秒)
                    size = (long)(wav.MixRate * (wav.Format == AudioStreamWav.FormatEnum.Format16Bits ? 2 : 1) * 
                                 (wav.Stereo ? 2 : 1) * wav.GetLength());
                    break;
                    
                case AudioStreamOggVorbis ogg:
                    // OGG大小估算(压缩音频)
                    size = (long)(ogg.GetLength() * 44100 * 0.1); // 估算压缩率
                    break;
                    
                case PackedScene scene:
                    // 场景大小估算 - 基于实例化后的节点数量
                    try 
                    {
                        var instance = scene.Instantiate<Node>();
                        var nodeCount = instance.GetChildCount();
                        size = nodeCount * 1024 * 5; // 每个节点估算5KB
                        instance.QueueFree();
                    }
                    catch 
                    {
                        size = 1024 * 100; // 回退到默认100KB
                    }
                    break;
                    
                default:
                    // 其他资源默认估算
                    size = 1024 * 10; // 10KB
                    break;
            }
            
            // 缓存结果
            if (!string.IsNullOrEmpty(resource.ResourcePath))
                _sizeCache[resource.ResourcePath] = size;
                
            return size;
        }

        public void ClearCache()
        {
            _sizeCache.Clear();
        }
    }
}
