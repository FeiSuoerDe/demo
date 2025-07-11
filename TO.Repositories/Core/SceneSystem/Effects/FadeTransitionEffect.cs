using Godot;
using GodotTask;
using TO.Repositories.Core.SceneSystem.Effects.Bases;

namespace TO.Repositories.Core.SceneSystem.Effects
{
    /// <summary>
    /// 淡入淡出过渡效果
    /// </summary>
    public class FadeTransitionEffect(bool isFadeIn) : BaseTransitionEffect
    {
        private Tween? _tween;
        
        public override async GDTask Execute(TransitionSystem system)
        {
            // 检查参数
            if (!Parameters.TryGetValue("time", out var value) || value is not float time)
            {
                time = 1.0f;
            }

            // 检查材质
            if (system.ColorRect.GetMaterial() is not ShaderMaterial material)
            {
                return;
            }
         
            system.CanvasLayer.Visible = true;

            _tween = system.SceneTree?.CreateTween();
            if (_tween == null)
            {
                return;
            }

            var startValue = isFadeIn ? 1.0f : 0.0f;
            var endValue = isFadeIn ? 0.0f : 1.0f;

            _tween.TweenProperty(material, "shader_parameter/dissolve_amount", endValue, time)
                .From(startValue);
            
            await _tween.ToSignal(_tween, Tween.SignalName.Finished);
            
            system.CanvasLayer.Visible = false;
            Dispose(true);
        }

        public override void Interrupt()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(_tween != null && !_tween.IsValid()) return;
            _tween?.Kill();
            _tween?.Dispose();
            _tween = null;
        }
    }
}