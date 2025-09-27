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
        // 鼠标离开：当存在并可见的武器选择列表时，不自动隐藏面板（由“点击面板外”来隐藏）
        if (_weaponSelectionList != null && _weaponSelectionList.Visible)
        {
            // 保持面板可见，但标记已不再 hover
            isMousExited = false;
            return;
        }

        // 否则按原逻辑：恢复半透明并隐藏详情面板与选择列表
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.8f);

        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = false;
        }

        if (_weaponSelectionList != null)
        {
            _weaponSelectionList.Visible = false;
        }

        isMousExited = false;
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

        // 确保武器选择列表在显示信息面板前被隐藏（或存在时复用但不可见）
        if (_weaponSelectionList != null)
        {
            _weaponSelectionList.Visible = false;
        }

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
        // 处理鼠标左键按下逻辑
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // 如果选择列表存在且可见：检测点击是否在面板内/外，面板外则隐藏两者
            if (_weaponSelectionList != null && _weaponSelectionList.Visible)
            {
                // 获取鼠标位置
                var mousePos = mouseEvent.Position;

                bool clickedInSelection = false;
                bool clickedInInfo = false;

                // 注意：使用 Control.GetGlobalRect() 判断点击是否在 Control 区域内（Godot 4）
                if (_weaponSelectionList is Control selControl)
                {
                    clickedInSelection = selControl.GetGlobalRect().HasPoint(mousePos);
                }
                if (_weaponInfoPanel != null && _weaponInfoPanel.Visible)
                {
                    clickedInInfo = _weaponInfoPanel.GetGlobalRect().HasPoint(mousePos);
                }

                if (!clickedInSelection && !clickedInInfo)
                {
                    _weaponSelectionList.Visible = false;
                    if (_weaponInfoPanel != null) _weaponInfoPanel.Visible = false;
                }

                return;
            }

            // 否则按原逻辑：只有当标记处于 hover 状态时才响应左键点击（打开选择列表）
            if (isMousExited)
            {
                OnLeftClick();
            }
        }
    }

    WeaponSelectionList _weaponSelectionList;

    // 在此方法中放置左键点击后的逻辑（示例：打印并确保信息面板可见）
    private void OnLeftClick()
    {
        if (PanelContainer == null) return;

        // 打开武器选择列表前，确保信息面板被隐藏（保持二者不会同时显示）
        if (_weaponInfoPanel != null)
        {
            _weaponInfoPanel.Visible = false;
        }

        // 如果已经存在选择列表实例，则直接显示（避免重复创建）
        if (_weaponSelectionList != null)
        {
            _weaponSelectionList.Visible = true;
            return;
        }

        var SelectionList = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["WeaponSeleList"]);
        if (SelectionList == null) return;

        _weaponSelectionList = SelectionList.Instantiate<WeaponSelectionList>();
        PanelContainer.AddChild(_weaponSelectionList);
        _weaponSelectionList.QueryWeapons(CurrentWeaponData.SpecificWeaponType, CurrentWeaponData.Size);
        _weaponSelectionList.Visible = true;

        // 可选：如果需要把当前武器传给选择列表，可在这里赋值（视WeaponSelectionList实现而定）
    }
}