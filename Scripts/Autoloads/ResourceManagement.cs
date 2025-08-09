using Godot;

// ResourceManagement.cs
namespace TimelapseInvoices.Scripts.Autoloads;

/// <summary>
/// 资源管理类，负责加载和管理游戏中的资源
/// /// 使用Godot的Resource系统来加载和管理资源
/// /// </summary>
public partial class ResourceManagement : Node
{
    // 武器资源列表
    [Export]
    public Resource.WeaponResource[] WeaponResources = new Resource.WeaponResource[] { };


}