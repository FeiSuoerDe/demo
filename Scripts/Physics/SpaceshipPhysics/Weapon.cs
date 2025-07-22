using Godot;
using System;

public partial class Weapon : Node2D
{
    // 武器类型枚举
    public enum WeaponType
    {
        ParticleCannon,
        Missile,
        Bullet,
        Beam
    }

    // 对应的中文名称数组
    public static readonly string[] WeaponTypeNames = {
        "粒子炮",
        "导弹",
        "实弹",
        "光束"
    };

    // 基本属性
    // 武器名称
    [Export]
    public string WeaponName = "DefaultWeapon";

    // 当前武器类型
    [Export]
    public WeaponType CurrentWeaponType = WeaponType.ParticleCannon;

    // 武器射速
    [Export]
    public float FireRate = 1.0f; // 每秒射击次数

    // 武器射程
    [Export]
    public float Range = 1000.0f;

    // 伤害属性
    // 武器伤害
    [Export]
    public int Damage = 10;

    // 对盾伤害倍率
    [Export]
    public float ShieldDamageMultiplier = 0.5f;

    // 对护甲伤害倍率
    [Export]
    public float ArmorDamageMultiplier = 1.0f;

    // 对生命值伤害倍率
    [Export]
    public float HealthDamageMultiplier = 1.0f;

    // 弹药相关
    // 载弹量
    [Export]
    public int AmmoCapacity = 100;

    // 当前弹药量
    [Export]
    public int CurrentAmmo = 100;

    // 换弹时间（秒）
    [Export]
    public float ReloadTime = 2.0f;

    // 计时器节点，用于处理换弹逻辑
    [Export]
    public Timer ReloadTimer;

    // 贴图地址
    [Export]
    public string TexturePath = "res://Textures/Weapons/DefaultWeapon.png";

    // 开火冷却跟踪
    private float _lastFireTime = 0f;
    private float _fireInterval => FireRate; // 计算射击间隔时间
    private bool _isReloading = false;

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
        ReloadTimer.WaitTime = ReloadTime;
        ReloadTimer.Timeout += OnReloadComplete;
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
            CurrentAmmo < AmmoCapacity)
        {
            StartReload();
        }
    }

    // 处理函数，用于连续开火
    public override void _Process(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Fire();
        }
    }

    // 开火方法
    public void Fire()
    {
        // 检查是否能开火
        if (_isReloading)
        {
            GD.Print($"{WeaponName}正在换弹中...");
            return;
        }

        if (CurrentAmmo <= 0)
        {
            GD.Print($"{WeaponName}弹药已耗尽!");
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
        CurrentAmmo--;

        GD.Print($"{WeaponName}开火! 剩余弹药: {CurrentAmmo}/{AmmoCapacity}");

        // 这里可以添加具体的开火效果代码，如生成子弹、粒子效果等
        // 根据武器类型执行不同的开火逻辑
        switch (CurrentWeaponType)
        {
            case WeaponType.ParticleCannon:
                // 粒子炮开火逻辑
                break;
            case WeaponType.Missile:
                // 导弹开火逻辑
                break;
            case WeaponType.Bullet:
                // 实弹开火逻辑
                break;
            case WeaponType.Beam:
                // 光束开火逻辑
                break;
        }

        // 当前弹药用完后自动换弹
        if (CurrentAmmo == 0)
        {
            GD.Print($"{WeaponName}弹药已用尽，开始自动换弹...");
            StartReload();
        }
    }

    // 开始换弹
    public void StartReload()
    {
        if (_isReloading || CurrentAmmo == AmmoCapacity)
        {
            return;
        }

        _isReloading = true;
        GD.Print($"{WeaponName}开始换弹，需要{ReloadTime}秒...");
        ReloadTimer.Start();
    }

    // 换弹完成的回调
    private void OnReloadComplete()
    {
        CurrentAmmo = AmmoCapacity;
        _isReloading = false;
        GD.Print($"{WeaponName}换弹完成! 弹药: {CurrentAmmo}/{AmmoCapacity}");
    }
}
