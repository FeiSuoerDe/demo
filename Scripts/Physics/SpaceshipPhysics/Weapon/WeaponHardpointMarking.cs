using Godot;
using TimelapseInvoices.Scripts.Autoloads;
using TimelapseInvoices.Scripts.Physics.SpaceshipPhysics.Weapon.WeaponHardpoint;

public partial class WeaponHardpointMarking : TextureButton
{
    // 鼠标进入时缩放为#ffffff5f,离开为F666
    public override void _Ready()
    {
        // 初始半透明
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);

        // 连接鼠标进入/离开事件
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseEntered()
    {
        // 鼠标进入：不透明并显示详情
        Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        ShowWeaponDetail();
    }

    private void OnMouseExited()
    {
        // 鼠标离开：恢复半透明并隐藏详情面板
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = false;
        }
    }

    // 武器信息面板
    private WeaponInfoPanel _weaponInfoPanel;

    // 面板存放处
    [Export]
    public Control PanelContainer;

    public void ShowWeaponDetail()
    {
        GD.Print("显示武器详情面板");
        if (_weaponInfoPanel == null && PanelContainer != null)
        {
            var panelScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["WeaponInfoPanel"]);
            if (panelScene != null)
            {
                _weaponInfoPanel = panelScene.Instantiate<WeaponInfoPanel>();
                // 查找父节点
                GD.Print("Parent Node: " + GetParent().Name);
                _weaponInfoPanel.weaponData = ((WeaponHardpoint)GetParent()).Data.weaponData;
                PanelContainer.AddChild(_weaponInfoPanel);
                _weaponInfoPanel.UpdateUI();
            }
        }

        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = true;
        }
    }

    // 检测点击外部区域关闭详情面板（使用面板的全局矩形判断）
    public override void _Input(InputEvent @event)
    {
        if (_weaponInfoPanel == null || !_weaponInfoPanel.Visible)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            Vector2 mousePosition = mouseEvent.Position;
            // 使用面板的全局矩形来判断点击是否在面板外部
            Rect2 panelRect = _weaponInfoPanel.GetGlobalRect();
            if (!panelRect.HasPoint(mousePosition))
            {
                _weaponInfoPanel.Visible = false;
            }
        }
    }
}
