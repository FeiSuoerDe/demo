using Godot;
using System;
using TimelapseInvoices.Scripts.DataClass;

public partial class DockShipCard : Control
{
    // 这里是船坞卡片
    ShipData shipData;
    [Export]
    // 舰船姓名
    Label ShipNameLabel;
    [Export]
    // 舰船型号
    Label ShipTypeLabel;
    // 数据初始化
    public void Init(ShipData data)
    {
        shipData = data;
        ShipNameLabel.Text = shipData.ShipName;
        ShipTypeLabel.Text = shipData.ShipModel;
    }

}
