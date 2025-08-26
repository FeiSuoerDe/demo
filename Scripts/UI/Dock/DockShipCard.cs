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
    Button panel;
    // 顺序
    public int index;

    // 船坞节点
    public Dock DockNode;
    // 数据初始化
    public override void _Ready()
    {
        // 设置Expandmode
        ShipTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        // Str
        ShipTexture.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        // 连接点击事件
        panel.Pressed += OnPanelPressed;
    }

    // 点击事件
    public void OnPanelPressed()
    {
        // 如果船坞节点未设置，打印错误
        if (DockNode == null)
        {
            GD.PrintErr("DockNode未设置，请在编辑器中设置。");
            return;
        }
        // 设置当前船坞的展示飞船
        DockNode.SetCurrentShipBody(index);
    }
    // 设置名字与型号接受两个字符串参数
    public void SetNameAndType(string name, string type)
    {
        if (ShipNameLabel != null)
        {
            ShipNameLabel.Text = name;
        }
        if (ShipTypeLabel != null)
        {
            ShipTypeLabel.Text = type;
        }
    }



}
