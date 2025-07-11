using GodotTask;

namespace TO.Repositories.Core.SceneSystem.Effects.Bases
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