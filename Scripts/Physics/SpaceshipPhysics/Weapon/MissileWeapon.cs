using System;
using Godot;
using TimelapseInvoices.Scripts.Autoloads;

namespace TimelapseInvoices.Scripts.Physics.SpaceshipPhysics.Weapon;

/// <summary>
/// 导弹武器类，实现导弹发射和追踪行为
/// 继承自基础武器类，提供导弹特有的装填、发射和追踪功能
/// </summary>
[GlobalClass]
public partial class MissileWeapon : Weapon
{
    [Export]
    public float TurnSpeed = 3.0f; // 转向速度，单位为弧度/秒，控制武器旋转跟随鼠标的速度

    // 导弹场景
    private PackedScene _missileScene; // 导弹预制体场景，用于实例化发射的导弹
    private double _nextFireTime = 0; // 下次可发射时间，基于武器射速计算
    private bool _isFiring; // 标记武器是否正在发射状态

    // 导弹武器特有的属性
    [Export]
    public int TotalAmmo = 20; // 总载弹量，表示备用弹药总数
    [Export]
    public int AmmoPerReload = 10; // 单次装填量，每次重新装填导弹的数量

    // 当前可用导弹数量
    private int _currentAmmo = 0; // 当前可用的弹药数量

    // 装填时间
    private float _currentReloadTime = 0f; // 当前装填计时器

    [Export]
    // 瞄准线，用于可视化导弹的发射方向
    public Line2D _aimLine;

    /// <summary>
    /// 初始化武器
    /// 设置武器类型、加载导弹资源及初始化瞄准线
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // 设置武器类型为导弹
        Data.SpecificWeaponType = DataClass.WeaponData.HardpointType.Missile;

        // 初始化当前弹药为一次装填量
        _currentAmmo = AmmoPerReload;

        // 加载导弹场景资源
        try
        {
            _missileScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["Rocket"]);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"无法加载导弹资源: {ex.Message}");
        }

        // 初始化瞄准线，设置宽度和两个点以显示射程
        if (_aimLine != null)
        {
            _aimLine.Width = 1;
            _aimLine.AddPoint(new Vector2(0, 0));
            _aimLine.AddPoint(new Vector2(Data.Range, 0));
            GD.Print($"导弹瞄准线初始化完成，射程: {Data.Range}");
        }
    }

    /// <summary>
    /// 处理武器更新逻辑
    /// 在每一帧更新武器的旋转、发射和换弹状态
    /// </summary>
    /// <param name="delta">帧间隔时间，用于平滑旋转计算</param>
    public override void _Process(double delta)
    {
        // 不调用基类的 _Process，因为基类有自己的装填逻辑
        // base._Process(delta);

        // 手动处理散布恢复逻辑（从基类复制）
        if (_currentSpread > Data.BaseSpreadAngle)
        {
            _currentSpread -= Data.SpreadRecoveryRate * (float)delta;
            if (_currentSpread < Data.BaseSpreadAngle)
            {
                _currentSpread = Data.BaseSpreadAngle;
            }
        }

        HandleWeaponRotation(delta); // 处理武器旋转，使其跟随鼠标
        HandleWeaponFiring(); // 处理武器发射逻辑
        HandleWeaponReload(); // 处理武器换弹逻辑

        // 处理装填倒计时
        if (_isReloading)
        {
            _currentReloadTime -= (float)delta;
            if (_currentReloadTime <= 0)
            {
                FinishReload();
            }
        }
    }

    /// <summary>
    /// 处理武器旋转，使武器朝向鼠标位置
    /// 通过插值实现平滑旋转效果
    /// </summary>
    /// <param name="delta">帧间隔时间，用于计算平滑旋转</param>
    private void HandleWeaponRotation(double delta)
    {
        var mousePosition = GetGlobalMousePosition(); // 获取鼠标全局坐标
        var direction = (mousePosition - GlobalPosition).Normalized(); // 计算武器到鼠标的方向向量
        var targetAngle = direction.Angle(); // 计算目标角度
        var currentAngle = GlobalRotation; // 获取当前角度
        GlobalRotation = Mathf.LerpAngle(currentAngle, targetAngle, TurnSpeed * (float)delta); // 使用线性插值平滑旋转
    }

    /// <summary>
    /// 处理武器射击逻辑
    /// 检测鼠标左键输入，在满足条件时发射导弹
    /// </summary>
    private void HandleWeaponFiring()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left) && !_isReloading)
        {
            bool ammoAvailable = _currentAmmo > 0; // 检查是否有可用弹药
            bool fireRateOK = GetTime() >= _nextFireTime; // 检查是否满足射速要求

            if (ammoAvailable && fireRateOK)
            {
                FireMissile(); // 发射导弹
                _currentAmmo--; // 减少当前弹药数量
                _nextFireTime = GetTime() + (1.0 / Data.FireRate); // 计算下次可发射时间

                // 当弹药耗尽且有备用弹药时自动换弹
                if (_currentAmmo <= 0 && TotalAmmo > 0)
                {
                    Reload();
                }
            }
        }
    }

    /// <summary>
    /// 处理武器换弹逻辑
    /// 检测鼠标右键输入，在满足条件时执行换弹操作
    /// </summary>
    private void HandleWeaponReload()
    {
        // 当按下鼠标右键、不在装填状态、弹药未满且有备用弹药时进行换弹
        if (Input.IsMouseButtonPressed(MouseButton.Right) && !_isReloading &&
            _currentAmmo < AmmoPerReload && TotalAmmo > 0)
        {
            Reload();
        }
    }

    /// <summary>
    /// 获取当前时间（秒）
    /// 用于计算武器射速和冷却时间
    /// </summary>
    /// <returns>当前时间（秒）</returns>
    private double GetTime()
    {
        return Time.GetTicksMsec() / 1000.0;
    }

    /// <summary>
    /// 基类Fire方法的实现，调用具体的射击逻辑
    /// 当满足发射条件时触发导弹发射
    /// </summary>
    public override void Fire()
    {
        if (_currentAmmo > 0 && !_isReloading)
        {
            FireMissile();
            _currentAmmo--;

            // 当弹药耗尽且有备用弹药时自动换弹
            if (_currentAmmo <= 0 && TotalAmmo > 0)
            {
                Reload();
            }
        }
    }

    /// <summary>
    /// 执行换弹操作
    /// 开始装填计时器，装填完成后会补充弹药
    /// </summary>
    public override void Reload()
    {
        // 当不在装填状态、弹药未满且有备用弹药时开始换弹
        if (!_isReloading && _currentAmmo < AmmoPerReload && TotalAmmo > 0)
        {
            _isReloading = true; // 设置装填状态
            _currentReloadTime = Data.ReloadTime; // 设置装填时间计时器

            GD.Print($"开始装填导弹，装填时间: {Data.ReloadTime}秒");
        }
    }

    /// <summary>
    /// 完成换弹操作
    /// 计算并更新弹药数量，从总弹药中扣除装填的数量
    /// </summary>
    protected void FinishReload()
    {
        _isReloading = false; // 重置装填状态

        // 只有当总载弹量大于0时才能装填
        if (TotalAmmo > 0)
        {
            // 计算实际可装填的弹药数量，不超过单次装填上限和总备用弹药量
            int reloadAmount = Math.Min(AmmoPerReload, TotalAmmo);

            // 更新当前弹药和总弹药
            _currentAmmo = reloadAmount; // 设置当前弹药为装填量
            TotalAmmo -= reloadAmount; // 从总弹药中扣除已装填的数量

            GD.Print($"导弹重新装填完成。当前弹药: {_currentAmmo}，剩余总弹药: {TotalAmmo}");
        }
        else
        {
            GD.Print("弹药耗尽，无法装填。");
        }
    }

    /// <summary>
    /// 发射导弹
    /// 实例化导弹对象，设置其初始位置、方向和伤害值
    /// </summary>
    private void FireMissile()
    {
        if (_missileScene == null)
        {
            GD.PrintErr("导弹场景未加载，无法发射导弹。");
            return;
        }

        // 从预制体实例化导弹对象
        Rocket.Rocket missile = _missileScene.Instantiate<Rocket.Rocket>();
        if (missile == null)
        {
            GD.PrintErr("导弹实例化失败。");
            return;
        }

        // 设置导弹的初始参数
        missile.GlobalPosition = FirePoint.GlobalPosition; // 设置发射位置为火力点位置
        missile.GlobalRotation = FirePoint.GlobalRotation; // 设置初始方向与火力点方向一致
        missile.Damage = Data.Damage; // 设置导弹伤害值

        // 将导弹添加到场景树的根节点
        GetTree().Root.AddChild(missile);

        GD.Print("导弹已发射！");
        // 输出当前载弹量和总载弹量
        GD.Print($"当前载弹量: {_currentAmmo}，总载弹量: {TotalAmmo}");
    }
    // GetStatus
    public override string GetStatus()
    {
        // 名称+载弹量/总载弹量
        return $"{Data.WeaponName} - {_currentAmmo}/{TotalAmmo}";
    }
}