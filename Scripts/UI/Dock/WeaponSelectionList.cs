using Godot;
using System;
using System.Collections.Generic;
using TimelapseInvoices.Scripts.Autoloads;
using TimelapseInvoices.Scripts.DataClass;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics.Weapon;

public partial class WeaponSelectionList : Panel
{
    // res://Scenes/UI/Dock/weapon_selection_list.tscn

    [Export]
    VBoxContainer vBoxContainer;
    // 查询方法，接受武器类型与大小参数
    public void QueryWeapons(WeaponData.HardpointType hardpointType, WeaponData.WeaponSize weaponSize)
    {
        GD.Print($"查询武器列表：槽位类型={hardpointType}, 武器大小={weaponSize}");
        List<WeaponData> filteredWeapons = new List<WeaponData>();
        // 更具武器类型查找
        switch (hardpointType)
        {

            case WeaponData.HardpointType.Kinetic:
                foreach (var weapon in GameManager.AllAutoWeaponScenes)
                {
                    var weaponInstance = weapon.Instantiate() as Node;
                    if (weaponInstance is AutocannonWeapon autoWeapon)
                    {
                        if (autoWeapon.Data.Size == weaponSize)
                        {
                            filteredWeapons.Add(autoWeapon.Data);

                        }
                    }
                }
                break;
            case WeaponData.HardpointType.Missile:
                foreach (var weapon in GameManager.AllMissileWeaponScenes)
                {
                    var weaponInstance = weapon.Instantiate() as Node;
                    if (weaponInstance is MissileWeapon missileWeapon)
                    {
                        if (missileWeapon.Data.Size == weaponSize)
                        {
                            filteredWeapons.Add(missileWeapon.Data);
                        }
                    }
                }
                break;


        }
        // 依照数据生成WeaponEntryPanel
        foreach (var item in filteredWeapons)
        {
            var panel = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["WeaponEntryPanel"]);
            WeaponEntryPanel weaponEntryPanel = panel.Instantiate<WeaponEntryPanel>();
            weaponEntryPanel.UpdateUI(item);
            vBoxContainer.AddChild(weaponEntryPanel);

        }



    }



}
