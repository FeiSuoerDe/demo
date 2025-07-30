using Godot;
using System;
using TimelapseInvoices.Scripts.Autoloads;

//命名空间
public partial class AutocannonWeapon: Weapon
{
    // 预加载的子弹场景
    private PackedScene _bulletScene;
    
    // 子弹实例
    private Projectile _bulletPrefab;
    
    // 重写初始化方法，加载实弹武器所需资源
    protected override void OnWeaponReady()
    {
        // 预加载子弹预制体以优化性能
        try
        {
            _bulletScene = ResourceLoader.Load<PackedScene>(NodeController.NodeDictionary["Projectile"]);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"无法加载子弹资源: {ex.Message}");
        }
    }
    
    // 实现实弹武器的发射逻辑
    protected override void OnWeaponFire(float spreadAngle)
    {
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
        _bulletPrefab.Rotation = Rotation + spreadAngle; // 使用当前武器的旋转角度加上随机散布

        // 将子弹添加到场景中
        GetTree().Root.AddChild(_bulletPrefab);
        
        // 这里可以添加实弹特有的效果，如弹壳抛出、枪口闪光等
    }
}