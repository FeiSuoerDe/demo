using Godot;
using System;
using TimelapseInvoices.Scripts.DataClass;

public partial class WeaponEntryPanel : Panel
{
    WeaponData weaponData;

    [Export]
    // 贴图
    TextureRect textureRect;
    [Export]
    // 名称
    Label nameLabel;
    [Export]
    // 费用
    Label costLabel;
    [Export]
    // 射程 
    Label rangeLabel;

    public void UpdateUI(WeaponData data)
    {
        weaponData = data;
        if (textureRect != null)
        {
            CompressedTexture2D compressedTexture2D = GD.Load<CompressedTexture2D>(data.TexturePath);
            textureRect.Texture = compressedTexture2D;
        }
        if (nameLabel != null)
        {
            nameLabel.Text = data.WeaponName;
        }
        if (costLabel != null)
        {
            costLabel.Text = $"费用: {data.Cost}";
        }
        if (rangeLabel != null)
        {
            rangeLabel.Text = $"射程: {data.Range} 伤害: {data.Damage}";
        }
    }

}
