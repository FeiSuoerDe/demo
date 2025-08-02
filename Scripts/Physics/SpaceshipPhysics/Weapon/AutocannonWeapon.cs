using System;
using Godot;
using TimelapseInvoices.Scripts.Autoloads;

/// <summary>
/// 实弹武器类，实现自动机枪行为
/// </summary>
public partial class AutocannonWeapon : Weapon
{
    // 转向角速度
    private const float TurnSpeed = 5.0f; // 转向速度，单位为弧度/秒

    // 子弹场景
    private PackedScene _bulletScene;
    private double _nextFireTime = 0; // 下次可发射时间
    private bool _isFiring;

    /// <summary>
    /// 初始化武器
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // 设置武器类型 (修正枚举引用)
        Data.SpecificWeaponType = WeaponData.HardpointType.Kinetic; // 设置为动能武器

        // 加载子弹场景
        try
        {
            _bulletScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["Projectile"]);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"无法加载子弹资源: {ex.Message}");
        }
    }

    /// <summary>
    /// 处理武器更新逻辑
    /// </summary>
    public override void _Process(double delta)
    {
        base._Process(delta); // 调用基类的处理方法，处理散布恢复和换弹计时

        HandleWeaponRotation(delta);
        HandleWeaponFiring();
        HandleWeaponReload();
    }

    /// <summary>
    /// 处理武器旋转，跟随鼠标位置
    /// </summary>
    private void HandleWeaponRotation(double delta)
    {
        // 获取鼠标在屏幕上的位置
        var mousePosition = GetGlobalMousePosition();
        // 计算从武器到鼠标的方向向量
        var direction = (mousePosition - GlobalPosition).Normalized();
        // 计算目标角度
        var targetAngle = direction.Angle();
        // 当前武器的角度
        var currentAngle = GlobalRotation;
        // 使用LerpAngle平滑地旋转武器
        GlobalRotation = Mathf.LerpAngle(currentAngle, targetAngle, TurnSpeed * (float)delta);
    }

    /// <summary>
    /// 处理武器射击逻辑
    /// </summary>
    private void HandleWeaponFiring()
    {
        // 检测鼠标左键状态
        if (Input.IsMouseButtonPressed(MouseButton.Left) && !_isReloading)
        {
            bool ammoAvailable = Data.CurrentAmmo > 0;
            bool fireRateOK = GetTime() >= _nextFireTime;

            if (ammoAvailable && fireRateOK)
            {
                FireProjectile();
                Data.CurrentAmmo--;
                // 设置下次可发射时间，基于射速
                _nextFireTime = GetTime() + (1.0 / Data.FireRate);
                IncreaseSpread();

                // 自动换弹
                if (Data.CurrentAmmo <= 0)
                {
                    Reload();
                }
            }
        }
    }

    /// <summary>
    /// 处理武器换弹逻辑
    /// </summary>
    private void HandleWeaponReload()
    {
        // 检测鼠标右键状态 (换弹)
        if (Input.IsMouseButtonPressed(MouseButton.Right) && !_isReloading && Data.CurrentAmmo < Data.AmmoCapacity)
        {
            Reload();
        }
    }

    /// <summary>
    /// 获取当前时间（秒）
    /// </summary>
    private double GetTime()
    {
        return Time.GetTicksMsec() / 1000.0;
    }

    /// <summary>
    /// 基类Fire方法的实现，调用具体的射击逻辑
    /// </summary>
    public override void Fire()
    {
        if (CanFire())
        {
            FireProjectile();
        }
    }

    /// <summary>
    /// 执行换弹操作
    /// </summary>
    public override void Reload()
    {
        if (!_isReloading && Data.CurrentAmmo < Data.AmmoCapacity)
        {
            _isReloading = true;
            _reloadTimer = Data.ReloadTime;
        }
    }

    /// <summary>
    /// 发射实弹
    /// </summary>
    private void FireProjectile()
    {
        if (_bulletScene == null)
        {
            GD.PrintErr("子弹场景未加载，无法发射子弹。");
            return;
        }

        // 实例化子弹
        Projectile bullet = _bulletScene.Instantiate<Projectile>();
        if (bullet == null)
        {
            GD.PrintErr("子弹实例化失败。");
            return;
        }

        // 设置子弹位置和方向（加入散布效果）
        bullet.GlobalPosition = FirePoint.GlobalPosition;
        bullet.GlobalRotation = FirePoint.GlobalRotation + GetSpreadRotation();

        // 添加到发射点
        FirePoint.AddChild(bullet);
    }
}