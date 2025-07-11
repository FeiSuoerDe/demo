using Godot;
using Godot.Collections;
using GodotTask;
using TO.Repositories.Abstractions.Core.UISystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;
using TO.Services.Core.UISystem.Animations;

namespace TO.Services.Core.UISystem;

public class UiAnimationService(IUiAnimationRepo uiAnimationRepo) : BaseService, IUiAnimationService
{
    /// <summary>
    /// 设置并获取控制节点的取消令牌
    /// </summary>
    private async GDTask<CancellationTokenSource> SetupCancellationToken(Control? control)
    {
        if (control == null) throw new ArgumentNullException(nameof(control));

        uiAnimationRepo.RemoveAnimation(control);
        
        var ctx = new CancellationTokenSource();
        uiAnimationRepo.AddAnimation(control, ctx);
        
        return ctx;
    }

 

    public async void MouseInOut(Control? control, 
        Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        Array<Variant>? args = null, 
        System.Collections.Generic.Dictionary<string, object>? kwargs = null)
    {
        if (control == null || args == null || kwargs == null || !kwargs.TryGetValue("isEntered", out var value)) 
            return;

        SetPivotOffset(control);
        var token = await SetupCancellationToken(control);
        try
        {
            var duration = args[0].As<float>();
            var scaleValue = (bool)value ? args[1].As<float>() : args[2].As<float>();

            await ScaleAnimation.ScaleAsync(
                control,
                control.Scale,
                Vector2.One * scaleValue,
                duration,
                ease,
                trans,
                token.Token
            );
            uiAnimationRepo.RemoveAnimation(control);
        }
        catch (OperationCanceledException)
        {
            
        }
        
    }

    public async void MousePressedReleased(Control? control, 
        Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        Array<Variant>? args = null, 
        System.Collections.Generic.Dictionary<string, object>? kwargs = null)
    {
        if (control == null || args == null || kwargs == null || !kwargs.ContainsKey("isPressed")) 
            return;

        SetPivotOffset(control);
        var token = await SetupCancellationToken(control);

        try
        {
            var duration = args[0].As<float>();
            var fromAlpha = control.Modulate.A;
            var toAlpha = (bool)(kwargs["isPressed"]) ? args[2].As<float>() : args[1].As<float>();

            await FadeAnimation.FadeAsync(
                control,
                fromAlpha,
                toAlpha,
                duration,
                ease,
                trans,
                token.Token
            );
        }
        catch (OperationCanceledException)
        {
           
        }
        finally
        {
            uiAnimationRepo.RemoveAnimation(control);
        }
    }

    public async void Popup(Control? control,
        Tween.EaseType ease = Tween.EaseType.Out,
        Tween.TransitionType trans = Tween.TransitionType.Cubic,
        Array<Variant>? args = null,
        System.Collections.Generic.Dictionary<string, object>? kwargs = null)
    {
        if (control == null || args == null || kwargs == null || !kwargs.ContainsKey("Visible")) 
            return;

        SetPivotOffset(control);
        var token = await SetupCancellationToken(control);

        try
        {
            var duration = args[0].As<float>();
            var isVisible = (bool)kwargs["Visible"];
            var fromPos = args[2].As<Vector2>();
            var toPos = args[1].As<Vector2>();

            if (isVisible)
            {
                control.Visible = true;
                await MoveAnimation.MoveAsync(
                    control,
                    fromPos,
                    toPos,
                    duration,
                    ease,
                    trans,
                    token.Token
                );
            }
            else
            {
                await MoveAnimation.MoveAsync(
                    control,
                    toPos,
                    fromPos,
                    duration,
                    ease,
                    trans,
                    token.Token
                );
                control.Visible = false;
            }
        }
        catch (OperationCanceledException)
        {
            
        }
        finally
        {
            uiAnimationRepo.RemoveAnimation(control);
        }
    }

    /// <summary>
    /// 设置控件的轴心偏移
    /// </summary>
    private void SetPivotOffset(Control? control, Vector2 offset = default)
    {
        if (offset == default) offset = new Vector2(0.5f, 0.5f);
        if (control != null)
            control.PivotOffset = new Vector2(control.Size.X * offset.X, control.Size.Y * offset.Y);
    }
}
