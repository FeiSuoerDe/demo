using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

public partial class Weapon : Node2D
{
    // 武器数据
    [Export]
    public WeaponData Data { get; set; } = new WeaponData();

    // 计时器节点，用于处理换弹逻辑
    [Export]
    public Timer ReloadTimer;

    // 开火冷却跟踪
    protected float _lastFireTime = 0f;
    protected float _fireInterval => Data.FireRate; // 计算射击间隔时间
    protected bool _isReloading = false;

    // 发射点
    [Export]
    public Node2D FirePoint;

    // 散布相关
    protected float _currentSpreadAngle = 0f; // 当前散布角度
    protected float _lastShotTime = 0f; // 上次射击时间，用于计算散布恢复
    protected readonly float _degToRad = (float)Math.PI / 180f; // 度转弧度的常量

    // 准备完成
    public override void _Ready()
    {
        // 初始化换弹计时器
        if (ReloadTimer == null)
        {
            ReloadTimer = new Timer();
            AddChild(ReloadTimer);
        }

        ReloadTimer.OneShot = true;
        ReloadTimer.WaitTime = Data.ReloadTime;
        ReloadTimer.Timeout += OnReloadComplete;

        // 初始化散布角度
        _currentSpreadAngle = Data.BaseSpreadAngle;
        
        // 调用子类初始化方法
        OnWeaponReady();
    }
    
    // 子类可重写的初始化方法
    protected virtual void OnWeaponReady()
    {
        // 子类中实现特定初始化逻辑
    }

    // 输入处理
    public override void _Input(InputEvent @event)
    {
        // 左键开火
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            Fire();
        }

        // R键换弹
        if (@event is InputEventKey keyEvent &&
            keyEvent.Keycode == Key.R &&
            keyEvent.Pressed &&
            !_isReloading &&
            Data.CurrentAmmo < Data.AmmoCapacity)
        {
            StartReload();
        }
    }

    // 处理函数，用于连续开火和武器旋转
    public override void _Process(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Fire();
        }

        // 如果不是自动武器，处理朝向鼠标方向的旋转
        if (!Data.IsAutomatic)
        {
            // 获取鼠标位置
            Vector2 mousePosition = GetGlobalMousePosition();

            // 计算当前朝向与目标朝向
            float targetRotation = (mousePosition - GlobalPosition).Angle();
            float currentRotation = Rotation;

            // 使用平滑旋转
            float rotationStep = Data.RotationSpeed * (float)delta;
            float angleDiff = Mathf.AngleDifference(currentRotation, targetRotation);
            float newRotation = currentRotation;

            if (Mathf.Abs(angleDiff) > rotationStep)
            {
                newRotation = currentRotation + (angleDiff > 0 ? rotationStep : -rotationStep);
            }
            else
            {
                newRotation = targetRotation;
            }

            // 设置旋转
            Rotation = newRotation;
        }

        // 处理散布恢复
        float currentTime = (float)Time.GetTicksMsec() / 1000f;
        if (currentTime - _lastShotTime > _fireInterval && _currentSpreadAngle > Data.BaseSpreadAngle)
        {
            _currentSpreadAngle -= Data.SpreadRecoveryRate * (float)delta;
            _currentSpreadAngle = Mathf.Max(_currentSpreadAngle, Data.BaseSpreadAngle);
        }
    }

    // 开火方法
    public virtual void Fire()
    {
        // 检查是否能开火
        if (_isReloading)
        {
            GD.Print($"{Data.WeaponName}正在换弹中...");
            return;
        }

        if (Data.CurrentAmmo <= 0)
        {
            GD.Print($"{Data.WeaponName}弹药已耗尽!");
            // 弹药耗尽时自动换弹
            StartReload();
            return;
        }

        // 检查射击冷却
        float currentTime = (float)Time.GetTicksMsec() / 1000f;
        if (currentTime - _lastFireTime < _fireInterval)
        {
            return; // 冷却中，不能开火
        }

        // 执行开火
        _lastFireTime = currentTime;
        _lastShotTime = _lastFireTime; // 记录最后射击时间用于散布恢复
        Data.CurrentAmmo--;

        // 应用散布
        _currentSpreadAngle = Mathf.Min(_currentSpreadAngle + Data.SpreadIncreasePerShot, Data.MaxSpreadAngle);
        float randomSpread = (float)GD.RandRange(-_currentSpreadAngle, _currentSpreadAngle) * _degToRad;
        
        // 调用子类实现的具体发射逻辑
        OnWeaponFire(randomSpread);

        GD.Print($"{Data.WeaponName}开火! 剩余弹药: {Data.CurrentAmmo}/{Data.AmmoCapacity}");

        // 当前弹药用完后自动换弹
        if (Data.CurrentAmmo == 0)
        {
            GD.Print($"{Data.WeaponName}弹药已用尽，开始自动换弹...");
            StartReload();
        }
    }
    
    // 子类需要实现的具体发射逻辑
    protected virtual void OnWeaponFire(float spreadAngle)
    {
        // 子类中实现具体武器类型的发射逻辑
    }

    // 开始换弹
    public virtual void StartReload()
    {
        if (_isReloading || Data.CurrentAmmo == Data.AmmoCapacity)
        {
            return;
        }

        _isReloading = true;
        GD.Print($"{Data.WeaponName}开始换弹，需要{Data.ReloadTime}秒...");
        ReloadTimer.Start();
    }

    // 换弹完成的回调
    protected virtual void OnReloadComplete()
    {
        Data.CurrentAmmo = Data.AmmoCapacity;
        _isReloading = false;
        GD.Print($"{Data.WeaponName}换弹完成! 弹药: {Data.CurrentAmmo}/{Data.AmmoCapacity}");
    }
}
