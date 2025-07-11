using Godot;
using Godot.Collections;

namespace TO.Services.Abstractions.Core.UISystem
{
    public interface IUiAnimationService
    {

        void MouseInOut(Control? control,
            Tween.EaseType ease = Tween.EaseType.Out,
            Tween.TransitionType trans = Tween.TransitionType.Cubic,
            Array<Variant>? args = null,
            System.Collections.Generic.Dictionary<string, object>? kwargs = null);
        void MousePressedReleased(Control? control,
            Tween.EaseType ease, Tween.TransitionType trans,
            Array<Variant>? args = null, 
            System.Collections.Generic.Dictionary<string, object>? kwargs = null);

        void Popup(Control? control,
            Tween.EaseType ease = Tween.EaseType.Out,
            Tween.TransitionType trans = Tween.TransitionType.Cubic,
            Array<Variant>? args = null,
            System.Collections.Generic.Dictionary<string, object>? kwargs = null);
        
    }
}