using System.Collections.Concurrent;
using Godot;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Repositories.Bases;

namespace TO.Repositories.Core.UISystem;

public class UiAnimationRepo : BaseRepo,IUiAnimationRepo
{
    private class AnimationEntry : IDisposable
    {
        private bool  _disposed;
        public CancellationTokenSource? Cts { get; init; }
        public Action? Unsubscribe { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Unsubscribe?.Invoke();
                Cts?.Cancel();
                Cts?.Dispose();
                Unsubscribe = null; // 断开引用
            }
            // 释放非托管资源
            _disposed = true;
        }
        ~AnimationEntry()
        {
            Dispose(false);
        }
    }

    private readonly ConcurrentDictionary<Control, AnimationEntry> _entries = new();

    /// <summary>
    /// 添加Control的动画令牌源
    /// </summary>
    public void AddAnimation(Control control, CancellationTokenSource cts)
    {
        // 清理旧记录（如果存在）
        if (_entries.TryRemove(control, out var oldEntry))
        {
            oldEntry.Dispose();
        }

        // 创建新记录
        var entry = new AnimationEntry { Cts = cts };

        // 订阅节点退出树事件
        control.TreeExiting += Handler;

        entry.Unsubscribe = () => control.TreeExiting -= Handler;
        _entries[control] = entry; // 线程安全写入
        return;

        // 定义事件处理器
        void Handler()
        {
            if (!_entries.TryRemove(control, out var value)) return;
            value.Dispose();
        }
    }

    /// <summary>
    /// 移除Control的动画令牌源（手动清理）
    /// </summary>
    public bool RemoveAnimation(Control control)
    {
        if (!_entries.TryRemove(control, out var entry)) return false;
        entry.Dispose();
        return true;
    }

    /// <summary>
    /// 获取Control的取消令牌
    /// </summary>
    public CancellationTokenSource? GetCancelToken(Control control)
    {
        return _entries.TryGetValue(control, out var entry) 
            ? entry.Cts 
            : null;
    }
}