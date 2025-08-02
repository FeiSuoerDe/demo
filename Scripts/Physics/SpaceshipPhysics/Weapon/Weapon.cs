using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

/// <summary>
/// 武器基类，提供所有武器通用的功能
/// </summary>
public partial class Weapon : Node2D
{
    // 武器数据
    [Export]
    public WeaponData Data;

    // 发射点
    [Export]
    public Node2D FirePoint { get; set; }

    // 计时器节点，用于处理换弹逻辑
    public Timer ReloadTimer;

    // 换弹计时器
    protected float _reloadTimer = 0f;

    // 是否正在换弹
    protected bool _isReloading = false;

    // 当前散布角度
    protected float _currentSpread = 0f;

    // 开火冷却跟踪
    private float _fireInterval => 1.0f / Data.FireRate; // 计算射击间隔时间

    // 预加载的子弹场景
    private PackedScene _bulletScene;

    /// <summary>
    /// 初始化武器组件
    /// </summary>
    public override void _Ready()
    {
        // 初始化当前散布为基础散布
        _currentSpread = Data.BaseSpreadAngle;
    }

    /// <summary>
    /// 输入处理
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // 默认实现为空，子类可以根据需要覆盖
    }

    /// <summary>
    /// 武器射击方法，由子类实现具体射击逻辑
    /// </summary>
    public virtual void Fire()
    {
        // 子类应该覆盖此方法以实现特定武器类型的射击逻辑
    }

    /// <summary>
    /// 武器换弹方法，由子类实现具体换弹逻辑
    /// </summary>
    public virtual void Reload()
    {
        // 子类应该覆盖此方法以实现特定武器类型的换弹逻辑
    }

    /// <summary>
    /// 基类处理通用的散布和换弹逻辑
    /// </summary>
    public override void _Process(double delta)
    {
        // 更新换弹计时器
        if (_isReloading)
        {
            _reloadTimer -= (float)delta;
            if (_reloadTimer <= 0)
            {
                _isReloading = false;
                Data.CurrentAmmo = Data.AmmoCapacity;
            }
        }

        // 散布恢复
        if (_currentSpread > Data.BaseSpreadAngle)
        {
            _currentSpread -= Data.SpreadRecoveryRate * (float)delta;
            if (_currentSpread < Data.BaseSpreadAngle)
            {
                _currentSpread = Data.BaseSpreadAngle;
            }
        }
    }

    /// <summary>
    /// 检查是否可以射击
    /// </summary>
    /// <returns>如果有弹药且不在换弹中返回true</returns>
    protected bool CanFire()
    {
        return Data.CurrentAmmo > 0 && !_isReloading;
    }

    /// <summary>
    /// 增加武器散布
    /// </summary>
    protected void IncreaseSpread()
    {
        _currentSpread += Data.SpreadIncreasePerShot;
        if (_currentSpread > Data.MaxSpreadAngle)
        {
            _currentSpread = Data.MaxSpreadAngle;
        }
    }

    /// <summary>
    /// 计算散布后的旋转角度
    /// </summary>
    /// <returns>散布后的旋转偏移量（弧度）</returns>
    protected float GetSpreadRotation()
    {
        return (float)(GD.RandRange(-_currentSpread, _currentSpread) * (Mathf.Pi / 180.0));
    }
}
