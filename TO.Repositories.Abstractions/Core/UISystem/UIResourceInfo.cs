namespace TO.Repositories.Abstractions.Core.UISystem;


/// <summary>
/// UI资源状态信息
/// </summary>
public class UIResourceInfo
{
    public bool IsLoaded { get; set; }
    public int ReferenceCount { get; set; }
}
