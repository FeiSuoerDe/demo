using Godot;
using System;
using TimelapseInvoices.Scripts.DataClass;

public partial class DockShipCard : Control
{
    // 这里是船坞卡片
    [Export]
    // 贴图
    public TextureRect ShipTexture = new TextureRect();
    ShipData shipData;
    [Export]
    // 舰船姓名
    Label ShipNameLabel;
    [Export]
    // 舰船型号
    Label ShipTypeLabel;
    [Export]
    // 点击事件载体panel
    Panel panel;
    // 数据初始化
    public override void _Ready()
    {
        // 设置Expandmode
        ShipTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        // Str
        ShipTexture.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
    }
    // 设置数据


}
