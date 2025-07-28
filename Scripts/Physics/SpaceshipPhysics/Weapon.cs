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
    private float _lastFireTime = 0f;
    private float _fireInterval => Data.FireRate; // 计算射击间隔时间
    private bool _isReloading = false;

    // 发射点
    [Export]
    public Node2D FirePoint;

    // 散布相关
    private float _currentSpreadAngle = 0f; // 当前散布角度
    private float _lastShotTime = 0f; // 上次射击时间，用于计算散布恢复
    private readonly float _degToRad = (float)Math.PI / 180f; // 度转弧度的常量

    // 预加载的子弹场景
    private PackedScene _bulletScene;

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

        // 预加载子弹预制体以优化性能
        try
        {

            _bulletScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["Projectile"]);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"无法加载子弹资源: {ex.Message}");
        }

        // 初始化散布角度
        _currentSpreadAngle = Data.BaseSpreadAngle;
    }

    // 子弹
    Projectile _bulletPrefab;

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

            // 另一种方法是直接使用LookAt，但需要注意Godot中2D的LookAt会立即设置旋转
            // LookAt(mousePosition);
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
    public void Fire()
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

        // 实例化子弹
        if (_bulletScene != null)
        {
            _bulletPrefab = _bulletScene.Instantiate<Projectile>();
        }
        else
        {
            _bulletPrefab = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["Projectile"]).Instantiate<Projectile>();
        }

        if (_bulletPrefab == null)
        {
            GD.PrintErr("无法加载子弹预制体");
            return;
        }

        // 设置子弹位置和方向（应用散布）
        _bulletPrefab.GlobalPosition = FirePoint.GlobalPosition;
        _bulletPrefab.Rotation = Rotation + randomSpread; // 使用当前武器的旋转角度加上随机散布

        // 将子弹添加到场景中
        GetTree().Root.AddChild(_bulletPrefab);

        GD.Print($"{Data.WeaponName}开火! 剩余弹药: {Data.CurrentAmmo}/{Data.AmmoCapacity}");

        // 这里可以添加具体的开火效果代码，如生成子弹、粒子效果等
        // 根据武器类型执行不同的开火逻辑
        switch (Data.CurrentWeaponType)
        {
            case WeaponData.WeaponType.ParticleCannon:
                // 粒子炮开火逻辑
                break;
            case WeaponData.WeaponType.Missile:
                // 导弹开火逻辑
                break;
            case WeaponData.WeaponType.Bullet:
                // 实弹开火逻辑
                break;
            case WeaponData.WeaponType.Beam:
                // 光束开火逻辑
                break;
        }

        // 当前弹药用完后自动换弹
        if (Data.CurrentAmmo == 0)
        {
            GD.Print($"{Data.WeaponName}弹药已用尽，开始自动换弹...");
            StartReload();
        }
    }

    // 开始换弹
    public void StartReload()
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
    private void OnReloadComplete()
    {
        Data.CurrentAmmo = Data.AmmoCapacity;
        _isReloading = false;
        GD.Print($"{Data.WeaponName}换弹完成! 弹药: {Data.CurrentAmmo}/{Data.AmmoCapacity}");
    }
}
