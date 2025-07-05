using Godot;
using GodotTask;
using inFras.Core.SceneSystem.Effects.Bases;

namespace inFras.Core.SceneSystem.Effects
{
    /// <summary>
    /// 淡入淡出过渡效果
    /// </summary>
    public class FadeTransitionEffect(bool isEntering) : BaseTransitionEffect
    {
        private Tween? _tween;
        
        public override async GDTask Execute(TransitionSystem system)
        {
            var time = GetParameter<float>("time", 1.0f);
            
            if (system.ColorRect.GetMaterial() is not ShaderMaterial material)
            {
                return;
            }
         
            system.CanvasLayer.Visible = true;

            if (!await ExecuteFadeTransition(system, material, time))
            {
                return;
            }
            
            system.CanvasLayer.Visible = false;
            Dispose(true);
        }
        
        /// <summary>
        /// 执行淡入淡出过渡动画
        /// </summary>
        private async GDTask<bool> ExecuteFadeTransition(TransitionSystem system, ShaderMaterial material, float time)
        {
            _tween = system.SceneTree?.CreateTween();
            if (_tween == null)
            {
                return false;
            }

            var (startValue, endValue) = isEntering ? (1.0f, 0.0f) : (0.0f, 1.0f);

            _tween.TweenProperty(material, "shader_parameter/dissolve_amount", endValue, time)
                .From(startValue);
            
            await _tween.ToSignal(_tween, Tween.SignalName.Finished);
            return true;
        }

        public override void Interrupt()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                CleanupTween();
            }
        }
        
        /// <summary>
        /// 清理Tween资源
        /// </summary>
        private void CleanupTween()
        {
            if (_tween != null && _tween.IsValid())
            {
                _tween.Kill();
                _tween.Dispose();
            }
            _tween = null;
        }
    }
}