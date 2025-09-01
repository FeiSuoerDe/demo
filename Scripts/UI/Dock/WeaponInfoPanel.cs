using Godot;
using System;
using TimelapseInvoices.Scripts.DataClass;

public partial class WeaponInfoPanel : Panel
{
    // 武器Data
    public WeaponData weaponData;

    // 设置武器data
    public void SetWeaponData(WeaponData weaponData)
    {
        weaponData = weaponData;
        UpdateUI();
    }
    // 更新UI
    public void UpdateUI()
    {
        // 获取子节点
        var weaponNameLabel = GetNode<Label>("WeaponName");
        var weaponTypeLabel = GetNode<Label>("WeaponType");
        var weaponDamageLabel = GetNode<Label>("WeaponDamage");
        var weaponRangeLabel = GetNode<Label>("WeaponRange");
        var weaponFireRateLabel = GetNode<Label>("WeaponFireRate");
        var weaponDescriptionLabel = GetNode<Label>("WeaponDescription");
        // 设置文本
        // 名字
        weaponNameLabel.Text = weaponData.WeaponName;
        // 类型
        weaponTypeLabel.Text = WeaponData.WeaponTypeNames[(int)weaponData.SpecificWeaponType];
        // 伤害
        weaponDamageLabel.Text = $"伤害: {weaponData.Damage}";
        // 射程
        weaponRangeLabel.Text = $"射程: {weaponData.Range} 米";
        // 射速
        weaponFireRateLabel.Text = $"射速: {weaponData.FireRate} 发/分钟";
        // 描述
        // weaponDescriptionLabel.Text = weaponData.Description;
    }
}
