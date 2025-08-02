using Godot;
using System;
using System.Collections.Generic;
// 飞船物理体
public partial class SpaceshipPhysics : RigidBody2D
{
    // 飞船状态枚举，与引擎状态对应
    public enum ShipState
    {
        Idle,      // 待机
        Off,       // 熄灭
        On,        // 开启
        Boosting   // 加速
    }

    // 加速倍率常量
    private const float BOOST_SPEED_MULTIPLIER = 2.0f;  // 加速状态下速度提升100%
    private const float BOOST_TURNING_MULTIPLIER = 2.0f;  // 加速状态下转向速度提升100%

    // 当前飞船状态
    private ShipState currentState = ShipState.Idle;

    // 是否处于加速状态
    private bool isBoosting = false;
    // 是否处于移动状态
    private bool isMoving = false;

    // shipData
    [Export]
    public ShipData ShipData; // 飞船数据

    // 武器list
    public List<Weapon> Weapons = new List<Weapon>(); // 飞船武器列表
                                                      // 引擎list
    public List<Engine> Engines = new List<Engine>(); // 飞船引擎列表

    [Export]
    // 速度标签
    public Label SpeedLabel; // 显示速度的标签
                             // 
    [Export]
    // 角速度标签
    public Label AngularSpeedLabel; // 显示角速度的标签
    [Export]
    // 角度
    public Label AngleLabel; // 显示角度的标签
    [Export]
    // 武器状态
    public Label WeaponStatusLabel; // 显示武器状态的标签

    [Export]
    // 引擎槽位节点
    public Node EngineMount; // 引擎槽位节点
    [Export]
    // 武器槽位node
    public Node WeaponHardpoint; // 武器槽位节点
    public bool IsControlled = false; // 是否受控

    public override void _Ready()
    {

        // 确保引擎初始状态正确
        UpdateEngines();
        // 获取所有武器和引擎
        GetWeapons();
        GetEngines();
        // 初始化飞船
        GD.Print("动力-子系统上线,引擎子系统上线,指挥权限移交.");
    }

    public override void _PhysicsProcess(double delta)
    {
        // 重置状态
        ResetStateForPhysicsUpdate();

        // 处理移动输入和计算力
        Vector2 force = HandleMovementInput();

        // 处理转向输入和计算扭矩
        float torque = HandleRotationInput();

        // 更新飞船状态
        UpdateShipState();

        // 应用物理力和扭矩
        ApplyCentralForce(force);
        ApplyTorque(torque);

        // 限制最大速度
        LimitMaxSpeed();

        // 限制最大角速度
        LimitMaxAngularVelocity();

        // 更新UI
        UpdateUI();
    }

    // 重置物理更新的状态
    private void ResetStateForPhysicsUpdate()
    {
        isMoving = false;
        isBoosting = false;
        // 更新当前速度
        ShipData.CurrentSpeed = LinearVelocity.Length();
    }

    // 处理移动输入并返回要应用的力
    private Vector2 HandleMovementInput()
    {
        Vector2 force = Vector2.Zero;
        bool isShiftPressed = Input.IsKeyPressed(Key.Shift);

        // 处理前进(W键)
        if (Input.IsKeyPressed(Key.W))
        {
            float accelerationForce = ShipData.Acceleration;
            if (isShiftPressed)
            {
                accelerationForce *= BOOST_SPEED_MULTIPLIER;
                isBoosting = true;
            }

            Vector2 forceDirection = -Transform.Y.Normalized();
            force += forceDirection * accelerationForce;
            isMoving = true;
        }

        // 处理后退(S键)
        if (Input.IsKeyPressed(Key.S))
        {
            float accelerationForce = ShipData.Acceleration;
            if (isShiftPressed)
            {
                accelerationForce *= BOOST_SPEED_MULTIPLIER;
                isBoosting = true;
            }

            Vector2 forceDirection = Transform.Y.Normalized();
            force += forceDirection * accelerationForce;
            GD.Print($"后退: 力量={accelerationForce}, 方向={forceDirection}");
            isMoving = true;
        }

        return force;
    }

    // 处理转向输入并返回要应用的扭矩
    private float HandleRotationInput()
    {
        float torque = 0f;
        bool isShiftPressed = Input.IsKeyPressed(Key.Shift);

        // 处理左转(Q键)
        if (Input.IsKeyPressed(Key.Q))
        {
            float turningForce = ShipData.TurningAcceleration * 10;
            if (isShiftPressed && isMoving)
            {
                turningForce *= BOOST_TURNING_MULTIPLIER;
            }
            torque -= turningForce;
        }

        // 处理右转(E键)
        if (Input.IsKeyPressed(Key.E))
        {
            float turningForce = ShipData.TurningAcceleration * 10;
            if (isShiftPressed && isMoving)
            {
                turningForce *= BOOST_TURNING_MULTIPLIER;
            }
            torque += turningForce;
        }

        return torque;
    }

    // 更新飞船状态
    private void UpdateShipState()
    {
        if (isMoving)
        {
            if (isBoosting)
            {
                SetShipState(ShipState.Boosting);
            }
            else
            {
                SetShipState(ShipState.On);
            }
        }
        else
        {
            SetShipState(ShipState.Idle);
        }
    }

    // 限制最大速度
    private void LimitMaxSpeed()
    {
        float currentMaxSpeed = ShipData.MaxSpeed;
        if (isBoosting)
        {
            currentMaxSpeed *= BOOST_SPEED_MULTIPLIER;
        }

        if (LinearVelocity.Length() > currentMaxSpeed)
        {
            LinearVelocity = LinearVelocity.Normalized() * currentMaxSpeed;
            ShipData.CurrentSpeed = currentMaxSpeed;
        }
    }

    // 限制最大角速度
    private void LimitMaxAngularVelocity()
    {
        float currentMaxTurningSpeed = ShipData.MaxTurningSpeed;
        if (isBoosting && isMoving)
        {
            currentMaxTurningSpeed *= BOOST_TURNING_MULTIPLIER;
        }

        if (Mathf.Abs(AngularVelocity) > currentMaxTurningSpeed)
        {
            AngularVelocity = Mathf.Sign(AngularVelocity) * currentMaxTurningSpeed;
        }
    }

    // 更新UI显示
    private void UpdateUI()
    {
        UpdateSpeedLabel();
        UpdateWeaponStatus();
        UpdateAngularSpeed();
        UpdateAngleLabel();
    }

    // 更新速度标签
    private void UpdateSpeedLabel()
    {
        if (SpeedLabel != null)
        {
            float currentSpeed = ShipData.CurrentSpeed;
            string speedText = $"速度: {currentSpeed:F1} px/s";

            if (isBoosting)
            {
                speedText += " [加速]";
            }

            float maxSpeedToUse = isBoosting ? ShipData.MaxSpeed * BOOST_SPEED_MULTIPLIER : ShipData.MaxSpeed;
            float speedPercent = (currentSpeed / maxSpeedToUse) * 100;
            speedText += $" ({speedPercent:F0}%)";

            SpeedLabel.Text = speedText;
        }
    }
    // 更新角度标签
    private void UpdateAngleLabel()
    {
        if (AngleLabel != null)
        {
            float angleInDegrees = Mathf.RadToDeg(Transform.Rotation);
            AngleLabel.Text = $"角度: {angleInDegrees:F1}°";
        }
    }
    // 获取所有武器
    public List<Weapon> GetWeapons()
    {
        if (WeaponHardpoint != null)
        {
            Weapons.Clear();
            foreach (Node child in WeaponHardpoint.GetChildren())
            {
                // 先检查子节点是否有自己的子节点
                if (child.GetChildCount() > 0)
                {
                    var weapon = child.GetChild<Weapon>(0);
                    if (weapon != null)
                    {
                        Weapons.Add(weapon);
                        GD.Print($"找到武器: {weapon.Data.WeaponName}");
                    }
                }
                else
                {
                    // 检查当前节点是否为武器类型
                    if (child is Weapon weapon)
                    {
                        Weapons.Add(weapon);
                        GD.Print($"找到直接武器: {weapon.Data.WeaponName}");
                    }
                    else
                    {
                        GD.Print($"武器挂载点 '{child.Name}' 下没有武器节点。");
                    }
                }
            }
        }
        return Weapons;
    }

    // 获取所有引擎
    public List<Engine> GetEngines()
    {
        if (EngineMount != null)
        {
            Engines.Clear();
            foreach (Node child in EngineMount.GetChildren())
            {
                if (child is Engine engine)
                {
                    Engines.Add(engine);
                }
            }
        }
        return Engines;
    }

    // 更新武器信息（名字+状态+剩余弹药/弹药上限）
    public void UpdateWeaponStatus()
    {
        if (WeaponStatusLabel != null && Weapons.Count > 0)
        {
            string statusText = "武器状态:\n";
            foreach (var weapon in Weapons)
            {
                if (weapon != null)
                {
                    statusText += $"{weapon.Data.WeaponName} -  剩余弹药: {weapon.Data.CurrentAmmo}/{weapon.Data.AmmoCapacity}\n";
                }
            }
            WeaponStatusLabel.Text = statusText;
        }
    }

    // 更新角速度
    public void UpdateAngularSpeed()
    {
        if (AngularSpeedLabel != null)
        {
            AngularSpeedLabel.Text = $"角速度: {AngularVelocity:F1} rad/s";
        }
    }

    // 设置飞船状态
    public void SetShipState(ShipState state)
    {
        if (currentState != state)
        {
            currentState = state;
            UpdateEngines();
        }
    }

    // 更新所有引擎的状态
    private void UpdateEngines()
    {
        if (Engines != null)
        {
            // 假设 ShipState 和 Engine.EngineState 的枚举值是对应的，可以直接转换
            Engine.EngineState engineState = (Engine.EngineState)currentState;
            foreach (var engine in Engines)
            {
                if (engine != null)
                {
                    engine.SetEngineState(engineState);
                }
            }
        }
    }
}


