using Godot;
using TimelapseInvoices.Scripts.DataClass;

namespace TimelapseInvoices.Scripts.Physics.SpaceshipPhysics;

/// <summary>
/// 飞船物理体，负责移动并挂在贴图
/// </summary>
[GlobalClass]
public partial class SpaceshipPhysics : RigidBody2D
{
    // 贴图节点
    [Export] public Sprite2D ShipSprite;
    // 碰撞体节点
    [Export] public CollisionPolygon2D ShipCollision;
    #region 枚举与常量
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

    // 物理控制相关常量
    private const float BOOST_SPEED_MULTIPLIER = 2.0f;     // 加速状态下速度提升倍率
    private const float BOOST_TURNING_MULTIPLIER = 2.0f;   // 加速状态下转向速度提升倍率
    private const float STRAFE_FORCE_RATIO = 0.5f;         // 平移力相对于前进力的比率
    private const float TURNING_FORCE_MULTIPLIER = 10f;    // 转向力乘数
    private const float ROTATION_DAMPING = 2f;             // 旋转阻尼系数
    private const float MIN_ANGULAR_VELOCITY = 0.1f;       // 最小角速度阈值
    #endregion

    #region 导出属性和字段
    [Export] public ShipData ShipData;                     // 飞船数据

    // 私有字段
    private ShipState currentState = ShipState.Idle;       // 当前飞船状态
    private bool isBoosting = false;                       // 是否处于加速状态
    private bool isMoving = false;                         // 是否处于移动状态

    // 公共属性
    public bool IsControlled { get; set; } = false;        // 是否受控
    #endregion

    #region Godot生命周期方法
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
    }
    #endregion

    #region 状态管理
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
        }
    }
    #endregion

    #region 运动控制
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
    #endregion

    #region 旋转控制
    /// <summary>
    /// 处理转向输入并返回要应用的扭矩
    /// </summary>
    private float HandleRotationInput()
    {
        return Input.IsKeyPressed(Key.Shift) ? HandleManualRotation() : HandleMouseRotation();
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
}