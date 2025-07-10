using Godot;
using GodotTask;

namespace TO.Repositories.Core.ResourceSystem
{
    internal class MemoryMonitor : IDisposable
    {
        private bool _isRunning;
        private bool _disposed;
        private readonly object _lock = new();
        
        public event Action<long>? OnMemoryPressure;
        public event Action? OnAutoReleaseTriggered;
        
        public long AutoReleaseThreshold { get; set; } = 800 * 1024 * 1024; // 800MB
        public float CheckInterval { get; set; } = 15.0f; // 15秒
        
        public void StartMonitoring()
        {
            lock (_lock)
            {
                if (_isRunning || _disposed) return;
                _isRunning = true;
                MonitoringTask().Forget();
            }
        }
        
        private async GDTask MonitoringTask()
        {
            GD.Print("[MemoryMonitor] 内存监控任务启动");
            
            try
            {
                while (!_disposed)
                {
                    await GDTask.Delay(TimeSpan.FromSeconds(CheckInterval));
                    
                    var memInfo = OS.GetMemoryInfo();
                    var usedMemory = memInfo["physical"].As<long>() - memInfo["free"].As<long>();
                    var memoryPressure = usedMemory / (double)memInfo["physical"];
                    
                    OnMemoryPressure?.Invoke(usedMemory);
                    
                    if (memoryPressure > 0.75)
                    {
                        GD.Print($"[MemoryMonitor] 内存压力 {memoryPressure:P0}, 触发自动释放");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        OnAutoReleaseTriggered?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[MemoryMonitor] 监控任务异常: {ex.Message}");
            }
            finally
            {
                GD.Print("[MemoryMonitor] 内存监控任务停止");
                _isRunning = false;
            }
        }
        public void SetThreshold(long bytes)
        {
            AutoReleaseThreshold = bytes;
        }
        
        public void SetInterval(float interval)
        {
            CheckInterval = interval;
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GD.Print("[MemoryMonitor] 已释放");
        }


        
    }
}
