using GodotTask;

namespace inFras.Core.SceneSystem.Effects.Bases
{
    /// <summary>
    /// 过渡效果基类
    /// </summary>
    public abstract class BaseTransitionEffect : ITransitionEffect, IDisposable
    {
        private bool _disposed = false;
        
        /// <summary>
        /// 效果参数字典
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
        
        /// <summary>
        /// 获取指定类型的参数值，如果不存在或类型不匹配则返回默认值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">参数键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值或默认值</returns>
        protected T GetParameter<T>(string key, T defaultValue = default!)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// 执行过渡效果
        /// </summary>
        public abstract GDTask Execute(TransitionSystem system);

        /// <summary>
        /// 中断效果
        /// </summary>
        public abstract void Interrupt();

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    Parameters?.Clear();
                }
                
                // 释放非托管资源
                _disposed = true;
            }
        }

        ~BaseTransitionEffect()
        {
            Dispose(false);
        }
    }
}