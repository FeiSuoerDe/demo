using Godot;
using TimelapseInvoices.Scripts.Autoloads;

// 传送门！
namespace TimelapseInvoices.Scripts.CosmicMap.Portal;

public partial class Portal : Node2D
{
    public int partialId; // 传送门的唯一标识符
    [Export]
    public Area2D area; // 传送门的区域

    // area 进入时触发
    public void _on_area_2d_body_entered(Node body)
    {
        Autoloads.GameManager.galaxies[partialId].PrintGalaxyInfo();
        PackedScene GalaxyDataDisplayUI = (PackedScene)ResourceLoader.Load(NodeController.NodeDictionary["GalaxyDataDisplayUI"]);
        if (GalaxyDataDisplayUI != null)
        {
            UI.GalaxyDataDisplayUI.GalaxyDataDisplayUi galaxyDataDisplayUiInstance = (UI.GalaxyDataDisplayUI.GalaxyDataDisplayUi)GalaxyDataDisplayUI.Instantiate();
            galaxyDataDisplayUiInstance.galaxyId = partialId; // 设置星系ID
            GetTree().Root.AddChild(galaxyDataDisplayUiInstance);
            galaxyDataDisplayUiInstance.Show();
            GD.Print("星系数据展示UI已显示，ID: " + partialId);
        }
        else
        {
            GD.PrintErr("无法加载星系数据展示UI场景");
        }


    }


}