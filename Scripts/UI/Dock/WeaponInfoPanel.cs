using Godot;
using System;
using TimelapseInvoices.Scripts.DataClass;

public partial class WeaponInfoPanel : Panel
{
    // 武器Data
    public WeaponData weaponData;

    // 直接暴露节点本身（在 Inspector 中直接拖拽对应 Label 节点）
    [Export]
    public Label WeaponName;
    [Export]
    public Label WeaponType;
    [Export]
    public Label WeaponDamage;
    [Export]
    public Label WeaponRange;
    [Export]
    public Label WeaponFireRate;
    [Export]
    public Label WeaponDescription;

    // _Ready 不再需要获取节点引用
    public override void _Ready()
    {
        // 不需要在运行时查找节点，已在编辑器中直接绑定
    }

    // 设置武器data
    public void SetWeaponData(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        UpdateUI();
    }

    // 更新UI
    public void UpdateUI()
    {
        if (weaponData == null) return;

        // 使用直接导出的节点字段
        WeaponName.Text = weaponData.WeaponName;
        WeaponType.Text = WeaponData.WeaponTypeNames[(int)weaponData.SpecificWeaponType];
        WeaponDamage.Text = $"伤害: {weaponData.Damage}";
        WeaponRange.Text = $"射程: {weaponData.Range} 米";
        WeaponFireRate.Text = $"射速: {weaponData.FireRate} 发/分钟";
        // WeaponDescription.Text = weaponData.Description;
    }
}
