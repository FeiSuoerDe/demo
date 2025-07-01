using Godot;

namespace TO.Domains.Models.Repositories.Abstractions.Core.UISystem;

public interface IUiAnimationRepo
{
    

    void AddAnimation(Control control, CancellationTokenSource cts);

    bool RemoveAnimation(Control control);

    CancellationTokenSource? GetCancelToken(Control control);
}