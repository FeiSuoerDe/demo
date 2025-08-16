using System;
using Godot;

// 武器槽位
namespace TimelapseInvoices.Scripts.Physics.SpaceshipPhysics.Weapon.WeaponHardpoint;


public class WeaponHardpointData
{
    // 在飞船贴图的相对位置
    public Vector2 ToPosition { get; set; } = new Vector2(0, 0);
    // 类型分为（能量，导弹，动能）
    public DataClass.WeaponData.HardpointType Type { get; set; } // 武器类型

    // 槽位大小（分为大中小特）
    public DataClass.WeaponData.WeaponSize Size { get; set; } // 槽位大小

    public WeaponHardpointData()
    {
        // 默认构造函数
    }

    public WeaponHardpointData(DataClass.WeaponData.HardpointType type, DataClass.WeaponData.WeaponSize size)
    {
        Type = type;
        Size = size;
    }

    // 复制数据的方法
    public WeaponHardpointData Clone()
    {
        return new WeaponHardpointData(Type, Size);
    }

    // 验证武器兼容性的方法
    public bool IsCompatibleWith(Weapon weapon)
    {
        return weapon.Data.SpecificWeaponType == Type && weapon.Data.Size == Size;
    }
}
[GlobalClass]

public partial class WeaponHardpoint : Node2D
{
    [Export]
    private DataClass.WeaponData.HardpointType _type;

    [Export]
    private DataClass.WeaponData.WeaponSize _size;

    // 数据对象
    public WeaponHardpointData Data { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        // 初始化数据
        Data = new WeaponHardpointData(_type, _size);

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
        // 输出自己的所有子节点
        for (int i = 0; i < GetChildCount(); i++)
        {
            GD.Print($"子节点 {i}: {GetChild(i).Name}");
        }
    }

    // 检查武器是否与槽位兼容
    public bool IsWeaponCompatible(Weapon weapon)
    {
        // 使用数据类的方法来检查兼容性
        bool isCompatible = Data.IsCompatibleWith(weapon);

        if (isCompatible)
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

    // 设置槽位类型
    public void SetHardpointType(DataClass.WeaponData.HardpointType type)
    {
        _type = type;
        Data.Type = type;
    }

    // 设置槽位大小
    public void SetHardpointSize(DataClass.WeaponData.WeaponSize size)
    {
        _size = size;
        Data.Size = size;
    }
}