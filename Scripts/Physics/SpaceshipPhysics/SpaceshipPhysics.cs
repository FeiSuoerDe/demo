using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 飞船物理体控制器
/// </summary>
public partial class SpaceshipPhysics : RigidBody2D
{
    #region 枚举定义
    /// <summary>
    /// 飞船状态枚举，与引擎状态对应
    /// </summary>
    public enum ShipState
    {
        Idle,      // 待机
        Off,       // 熄灭
        On,        // 开启
        Boosting   // 加速
    }
    #endregion

    #region 常量定义
    private const float BOOST_SPEED_MULTIPLIER = 2.0f;     // 加速状态下速度提升倍率
    private const float BOOST_TURNING_MULTIPLIER = 2.0f;   // 加速状态下转向速度提升倍率
    private const float STRAFE_FORCE_RATIO = 0.5f;         // 平移力相对于前进力的比率
    private const float TURNING_FORCE_MULTIPLIER = 10f;    // 转向力乘数
    private const float ROTATION_DAMPING = 2f;             // 旋转阻尼系数
    private const float MIN_ANGULAR_VELOCITY = 0.1f;       // 最小角速度阈值
    #endregion

    #region 导出属性
    [Export] public ShipData ShipData;                     // 飞船数据
    [Export] public Label SpeedLabel;                      // 速度显示标签
    [Export] public Label AngularSpeedLabel;               // 角速度显示标签
    [Export] public Label AngleLabel;                      // 角度显示标签
    [Export] public Label WeaponStatusLabel;               // 武器状态显示标签
    [Export] public Node EngineMount;                      // 引擎槽位节点
    [Export] public Node WeaponHardpoint;                  // 武器槽位节点
    #endregion

    #region 私有字段
    private ShipState currentState = ShipState.Idle;       // 当前飞船状态
    private bool isBoosting = false;                       // 是否处于加速状态
    private bool isMoving = false;                         // 是否处于移动状态
    #endregion

    #region 公共属性
    public List<Weapon> Weapons = new List<Weapon>();      // 飞船武器列表
    public List<Engine> Engines = new List<Engine>();      // 飞船引擎列表
    public bool IsControlled = false;                      // 是否受控
    #endregion

    #region Godot生命周期
    public override void _Ready()
    {
        InitializeSpaceship();
    }

    public override void _PhysicsProcess(double delta)
    {
        // 重置状态
        ResetStateForPhysicsUpdate();

        // 处理输入和计算物理力
        Vector2 force = HandleMovementInput();
        float torque = HandleRotationInput();

        // 更新状态和应用物理
        UpdateShipState();
        ApplyCentralForce(force);
        ApplyTorque(torque);

        // 限制速度
        LimitMaxSpeed();
        LimitMaxAngularVelocity();

        // 更新UI
        UpdateUI();
    }
    #endregion

    #region 初始化方法
    /// <summary>
    /// 初始化飞船系统
    /// </summary>
    private void InitializeSpaceship()
    {
        UpdateEngines();
        GetWeapons();
        GetEngines();
        GD.Print("动力子系统上线，引擎子系统上线，指挥权限移交。");
    }
    #endregion

    #region 物理更新相关
    /// <summary>
    /// 重置物理更新的状态
    /// </summary>
    private void ResetStateForPhysicsUpdate()
    {
        isMoving = false;
        isBoosting = false;
        ShipData.CurrentSpeed = LinearVelocity.Length();
    }

    /// <summary>
    /// 处理移动输入并返回要应用的力
    /// </summary>
    private Vector2 HandleMovementInput()
    {
        Vector2 force = Vector2.Zero;
        float accelerationForce = ShipData.Acceleration;

        // 前进(W键)
        if (Input.IsKeyPressed(Key.W))
        {
            force += -Transform.Y.Normalized() * accelerationForce;
            isMoving = true;
        }

        // 后退(S键)
        if (Input.IsKeyPressed(Key.S))
        {
            force += Transform.Y.Normalized() * accelerationForce;
            isMoving = true;
        }

        // 左平移(A键)
        if (Input.IsKeyPressed(Key.A))
        {
            float strafeForce = accelerationForce * STRAFE_FORCE_RATIO;
            force += -Transform.X.Normalized() * strafeForce;
            isMoving = true;
        }

        // 右平移(D键)
        if (Input.IsKeyPressed(Key.D))
        {
            float strafeForce = accelerationForce * STRAFE_FORCE_RATIO;
            force += Transform.X.Normalized() * strafeForce;
            isMoving = true;
        }

        return force;
    }

    /// <summary>
    /// 处理转向输入并返回要应用的扭矩
    /// </summary>
    private float HandleRotationInput()
    {
        bool isShiftPressed = Input.IsKeyPressed(Key.Shift);

        if (isShiftPressed)
        {
            return HandleManualRotation();
        }

        return HandleMouseRotation();
    }

    /// <summary>
    /// 处理手动旋转(Shift+Q/E)
    /// </summary>
    private float HandleManualRotation()
    {
        float torque = 0f;
        float turningForce = ShipData.TurningAcceleration * TURNING_FORCE_MULTIPLIER;

        if (Input.IsKeyPressed(Key.Q))
        {
            torque -= turningForce;
        }

        if (Input.IsKeyPressed(Key.E))
        {
            torque += turningForce;
        }

        // 应用旋转阻尼
        if (torque == 0f && Mathf.Abs(AngularVelocity) > MIN_ANGULAR_VELOCITY)
        {
            return -AngularVelocity * ROTATION_DAMPING;
        }

        return torque;
    }

    /// <summary>
    /// 处理鼠标方向旋转
    /// </summary>
    private float HandleMouseRotation()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        float targetAngle = GlobalPosition.AngleToPoint(mousePosition) + Mathf.Pi / 2;
        float currentAngle = Rotation;

        float angleDifference = Mathf.Wrap(targetAngle - currentAngle, -Mathf.Pi, Mathf.Pi);
        return angleDifference * ShipData.TurningAcceleration * TURNING_FORCE_MULTIPLIER;
    }

    /// <summary>
    /// 限制最大速度
    /// </summary>
    private void LimitMaxSpeed()
    {
        float currentMaxSpeed = ShipData.MaxSpeed;

        if (LinearVelocity.Length() > currentMaxSpeed)
        {
            LinearVelocity = LinearVelocity.Normalized() * currentMaxSpeed;
            ShipData.CurrentSpeed = currentMaxSpeed;
        }
    }

    /// <summary>
    /// 限制最大角速度
    /// </summary>
    private void LimitMaxAngularVelocity()
    {
        float currentMaxTurningSpeed = ShipData.MaxTurningSpeed;

        if (Mathf.Abs(AngularVelocity) > currentMaxTurningSpeed)
        {
            AngularVelocity = Mathf.Sign(AngularVelocity) * currentMaxTurningSpeed;
        }
    }
    #endregion

    #region 状态管理
    /// <summary>
    /// 更新飞船状态
    /// </summary>
    private void UpdateShipState()
    {
        ShipState newState = isMoving ? (isBoosting ? ShipState.Boosting : ShipState.On) : ShipState.Idle;
        SetShipState(newState);
    }

    /// <summary>
    /// 设置飞船状态
    /// </summary>
    public void SetShipState(ShipState state)
    {
        if (currentState != state)
        {
            currentState = state;
            UpdateEngines();
        }
    }

    /// <summary>
    /// 更新所有引擎的状态
    /// </summary>
    private void UpdateEngines()
    {
        if (Engines != null)
        {
            Engine.EngineState engineState = (Engine.EngineState)currentState;
            foreach (var engine in Engines)
            {
                engine?.SetEngineState(engineState);
            }
        }
    }
    #endregion

    #region 组件获取
    /// <summary>
    /// 获取所有武器
    /// </summary>
    public List<Weapon> GetWeapons()
    {
        if (WeaponHardpoint == null) return Weapons;

        Weapons.Clear();
        foreach (Node child in WeaponHardpoint.GetChildren())
        {
            Weapon weapon = GetWeaponFromNode(child);
            if (weapon != null)
            {
                Weapons.Add(weapon);
                GD.Print($"找到武器: {weapon.Data.WeaponName}");
            }
            else
            {
                GD.Print($"武器挂载点 '{child.Name}' 下没有武器节点。");
            }
        }
        return Weapons;
    }

    /// <summary>
    /// 从节点获取武器组件
    /// </summary>
    private Weapon GetWeaponFromNode(Node node)
    {
        // 检查子节点
        if (node.GetChildCount() > 0)
        {
            return node.GetChild<Weapon>(0);
        }

        // 检查当前节点
        return node as Weapon;
    }

    /// <summary>
    /// 获取所有引擎
    /// </summary>
    public List<Engine> GetEngines()
    {
        if (EngineMount == null) return Engines;

        Engines.Clear();
        foreach (Node child in EngineMount.GetChildren())
        {
            if (child is Engine engine)
            {
                Engines.Add(engine);
            }
        }
        return Engines;
    }
    #endregion

    #region UI更新
    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        UpdateSpeedLabel();
        UpdateWeaponStatus();
        UpdateAngularSpeed();
        UpdateAngleLabel();
    }

    /// <summary>
    /// 更新速度标签
    /// </summary>
    private void UpdateSpeedLabel()
    {
        if (SpeedLabel == null) return;

        float currentSpeed = ShipData.CurrentSpeed;
        float speedPercent = (currentSpeed / ShipData.MaxSpeed) * 100;
        SpeedLabel.Text = $"速度: {currentSpeed:F1} px/s ({speedPercent:F0}%)";
    }

    /// <summary>
    /// 更新角度标签
    /// </summary>
    private void UpdateAngleLabel()
    {
        if (AngleLabel == null) return;

        float angleInDegrees = Mathf.RadToDeg(Transform.Rotation);
        AngleLabel.Text = $"角度: {angleInDegrees:F1}°";
    }

    /// <summary>
    /// 更新武器状态
    /// </summary>
    public void UpdateWeaponStatus()
    {
        if (WeaponStatusLabel == null || Weapons.Count == 0) return;

        string statusText = "武器状态:\n";
        foreach (var weapon in Weapons)
        {
            if (weapon != null)
            {
                statusText += $"{weapon.GetStatus()}\n";
            }
        }
        WeaponStatusLabel.Text = statusText;
    }

    /// <summary>
    /// 更新角速度显示
    /// </summary>
    public void UpdateAngularSpeed()
    {
        if (AngularSpeedLabel == null) return;

        AngularSpeedLabel.Text = $"角速度: {AngularVelocity:F1} rad/s";
    }
    #endregion
}


