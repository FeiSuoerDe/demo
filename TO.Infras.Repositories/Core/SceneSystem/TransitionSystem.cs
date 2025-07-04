using Godot;
using GodotTask;
using inFras.Core.SceneSystem.Effects.Bases;
using TO.Commons.Enums;

namespace inFras.Core.SceneSystem
{
    /// <summary>
    /// 过渡效果管理系统
    /// </summary>
    public class TransitionSystem(CanvasLayer canvasLayer, ColorRect colorRect, SceneTree? sceneTree)
    {
        public CanvasLayer CanvasLayer { get; } = canvasLayer;
        public ColorRect ColorRect { get; } = colorRect;
        public SceneTree? SceneTree { get; } = sceneTree;
        private readonly Queue<ITransitionEffect> _effectQueue = new();
        
        /// <summary>
        /// 当前过渡状态
        /// </summary>
        private TransitionState _currentState = TransitionState.Idle;
        public TransitionState CurrentState 
        { 
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnStateChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// 状态变化事件
        /// </summary>
        public event Action<TransitionState>? OnStateChanged;

        /// <summary>
        /// 添加效果到队列
        /// </summary>
        public void EnqueueEffect(ITransitionEffect effect)
        {
            _effectQueue.Enqueue(effect);
        }

        /// <summary>
        /// 执行所有队列中的效果
        /// </summary>
        /// <param name="forceRestart">是否强制重启当前过渡</param>
        public async GDTask ExecuteAll(bool forceRestart = false)
        {
            // 处理非空闲状态
            if (CurrentState != TransitionState.Idle)
            {
                if (forceRestart)
                {
                    await SafeReset();
                }
                else
                {
                    return;
                }
            }

            try
            {
                CurrentState = TransitionState.Running;

                while (_effectQueue.Count > 0 && CurrentState == TransitionState.Running)
                {
                    var effect = _effectQueue.Dequeue();
                    
                    try
                    {
                        await effect.Execute(this);
                    }
                    catch (Exception)
                    {
                        await SafeReset();
                        throw;
                    }
                }

                CurrentState = TransitionState.Completed;
            }
            finally
            {
                // 确保状态最终被重置
                if (CurrentState == TransitionState.Running)
                {
                    await SafeReset();
                }
                else
                {
                    CurrentState = TransitionState.Idle;
                }
            }
        }

        /// <summary>
        /// 安全重置过渡系统状态
        /// </summary>
        private async GDTask SafeReset()
        {
            // 中断当前效果
            InterruptEffects();
            
            // 重置状态
            CurrentState = TransitionState.Idle;
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 中断所有效果但不改变状态
        /// </summary>
        private void InterruptEffects()
        {
            foreach (var effect in _effectQueue)
            {
                effect.Interrupt();
            }
            _effectQueue.Clear();
        }

        /// <summary>
        /// 中断当前过渡
        /// </summary>
        public void Interrupt()
        {
            if (CurrentState != TransitionState.Running) return;

            CurrentState = TransitionState.Interrupted;
            InterruptEffects();
        }
    }
}
