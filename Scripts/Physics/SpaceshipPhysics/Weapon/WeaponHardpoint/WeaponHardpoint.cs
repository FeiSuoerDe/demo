using Godot;
using System;

// 武器槽位
public partial class WeaponHardpoint : Node2D
{
    // 类型分为（能量，导弹，动能）
    [Export]
    public WeaponData.HardpointType Type; // 武器类型
                                          // 槽位大小（分为大中小特）
    [Export]
    public WeaponData.WeaponSize Size; // 槽位大小
    public override void _Ready()
    {
        base._Ready();

        try
        {
            // 检查是否有子节点
            if (GetChildCount() > 0)
            {
                var weaponNode = GetChild<Weapon>(0);
                if (weaponNode != null)
                {
                    if (IsWeaponCompatible(weaponNode))
                    {
                        GD.Print("武器已就位，准备就绪。");
                    }
                    else
                    {
                        GD.Print("武器不兼容，已被移除。");
                    }
                }
                else
                {
                    GD.Print("挂载点的第一个子节点不是武器类型。");
                }
            }
            else
            {
                GD.Print("未检测到武器节点，槽位空闲。");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"检查武器兼容性时发生错误: {ex.Message}");
        }
    }
    // 检查武器是否与槽位兼容
    public bool IsWeaponCompatible(Weapon weapon)
    {
        // 检查武器类型与槽位类型是否匹配
        // 并且检查武器大小与槽位大小是否匹配
        // 如果不匹配则删除武器
        if (weapon.Data.SpecificWeaponType == Type && weapon.Data.Size == Size)
        {
            // 输出确认文本,高科技风格(中文)
            GD.Print($"武器 {weapon.Data.WeaponName} 与此槽位兼容-子系统就位");
            return true; // 兼容
        }
        else
        {
            GD.Print($"武器 {weapon.Data.WeaponName} 与此槽位不兼容-子系统失效");
            weapon.QueueFree(); // 删除不兼容的武器
            return false; // 不兼容
        }
    }
}
