using Godot;
using TimelapseInvoices.Scripts.Autoloads;
using TimelapseInvoices.Scripts.DataClass;
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
    // 鼠标是否进入
    public bool isMousExited = false;

    private void OnMouseEntered()
    {
        // 鼠标进入：不透明并显示详情
        Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        ShowWeaponDetail();
        isMousExited = true;
    }

    private void OnMouseExited()
    {
        // 鼠标离开：恢复半透明并隐藏详情面板
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = false;
            isMousExited = false;
        }

    }

    // 武器信息面板
    private WeaponInfoPanel _weaponInfoPanel;

    // 面板存放处
    [Export]
    public Control PanelContainer;

    // 当前武器数据
    public WeaponData CurrentWeaponData;
    public void ShowWeaponDetail()
    {
        GD.Print("显示武器详情面板");
        if (_weaponInfoPanel == null && PanelContainer != null)
        {
            var panelScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["WeaponInfoPanel"]);
            if (panelScene != null)
            {
                _weaponInfoPanel = panelScene.Instantiate<WeaponInfoPanel>();
                _weaponInfoPanel.weaponData = CurrentWeaponData;
                PanelContainer.AddChild(_weaponInfoPanel);
                _weaponInfoPanel.UpdateUI();
            }
        }

        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = true;
        }
    }

    // 检测点击事件
    public override void _Input(InputEvent @event)
    {
        // 是否进入
        if (isMousExited)
        {
            // 按下鼠标左键
            if (@event is InputEventMouseButton mouseEvent &&
                mouseEvent.Pressed &&
                mouseEvent.ButtonIndex == MouseButton.Left)
            {
                GD.Print("检测到左键按下");
                OnLeftClick();
                // 标记事件已处理，防止继续向下传递（需要时启用）
            }
        }
    }

    // 在此方法中放置左键点击后的逻辑（示例：打印并确保信息面板可见）
    private void OnLeftClick()
    {
        GD.Print("WeaponHardpoint 被左键点击");
        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = true;
        }
        // 其它点击逻辑可在此扩展（选中、切换武器等）
    }
}