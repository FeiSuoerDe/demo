using System.Collections.Generic;
using Godot;

namespace TimelapseInvoices.Scripts.Autoloads;

/// <summary>
/// 节点控制器
/// </summary>
public partial class NodeController : Node
{
    // 单例实例
    public static NodeController Instance { get; private set; }

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            GD.Print("加载 NodeController 实例");
        }
        else
        {
            GD.PrintErr("NodeController 实例已存在，无法创建新的实例。");
            QueueFree(); // Remove this instance if another already exists
        }
    }

    // 节点路径字典
    public static Dictionary<string, string> NodeDictionary { get; private set; } = new Dictionary<string, string>()
    {
        { "GameManager", "GameManager" },
        { "Galaxy", "res://Scenes/Galaxy/galaxy.tscn" },
        { "Planet", "res://Scenes/Galaxy/Planet/planet.tscn" },
        { "MainCamera", "res://Scenes/Camera/main_camera_2d.tscn" },
        { "CosmicMap", "res://Scenes/CosmicMap/cosmic_map.tscn" },
        { "Portal", "res://Scenes/CosmicMap/Portal/portal.tscn" },
        { "MapContShip", "res://Scenes/CosmicMap/MapContShip/map_cont_ship.tscn" },
        { "GalaxyDataDisplayUI", "res://Scenes/UI/GalaxyDataDisplayUI/galaxy_data_display_ui.tscn" },
        { "PlanetInfoItem", "res://Scenes/UI/GalaxyDataDisplayUI/planet_info_item.tscn" },
        { "Projectile", "res://Scenes/Physics/Projectile/projectile.tscn" },
        { "Rocket", "res://Scenes/Physics/Rocket/rocket.tscn" },
        { "WeaponHardpointMarking", "res://Scenes/Physics/SpaceshipPhysics/weapon/WeaponHardpointMarking.tscn" },
        {"DockShipCard","res://Scenes/UI/Dock/dock_ship_card.tscn"},
        {"WeaponInfoPanel","res://Scenes/UI/Dock/WeaponInfoPanel.tscn"}
    };
}