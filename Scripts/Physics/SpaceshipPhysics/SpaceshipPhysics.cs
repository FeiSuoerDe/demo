using Godot;
using System;
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

    // shipData
    [Export]
    public ShipData ShipData; // 飞船数据

    public override void _Ready()
    {
        // 初始化飞船
        GD.Print("Spaceship is ready.");
        // 确保引擎初始状态正确
        UpdateEngines();
    }
    [Export]
    // 速度标签
    public Label SpeedLabel; // 显示速度的标签
    public override void _PhysicsProcess(double delta)
    {
        var force = Vector2.Zero;
        var torque = 0f;
        bool isMoving = false;

        // 检测是否按下Shift键(加速键)
        bool isShiftPressed = Input.IsKeyPressed(Key.Shift);
        isBoosting = false; // 重置加速状态

        // 获取当前速度并更新ShipData
        float currentSpeed = LinearVelocity.Length();
        ShipData.CurrentSpeed = currentSpeed;

        // W/S 控制前进后退
        if (Input.IsKeyPressed(Key.W)) // W - 前进
        {
            // 计算前进力度
            float accelerationForce = ShipData.Acceleration;
            if (isShiftPressed)
            {
                // Shift+W 进入加速状态，增强推力
                accelerationForce *= BOOST_SPEED_MULTIPLIER;
                isBoosting = true;
            }

            // 在飞船的局部Y轴负方向应用力 (前进方向)
            Vector2 forceDirection = -Transform.Y.Normalized();
            force += forceDirection * accelerationForce;

            // 确保仪表板显示正确
            GD.Print($"前进: 力量={accelerationForce}, 方向={forceDirection}");

            isMoving = true;
        }

        if (Input.IsKeyPressed(Key.S)) // S - 后退
        {
            // 计算后退力度
            float accelerationForce = ShipData.Acceleration;
            if (isShiftPressed)
            {
                // Shift+S 进入加速状态，增强推力
                accelerationForce *= BOOST_SPEED_MULTIPLIER;
                isBoosting = true;
            }

            // 在飞船的局部Y轴正方向应用力 (后退方向)
            Vector2 forceDirection = Transform.Y.Normalized();
            force += forceDirection * accelerationForce;

            // 确保仪表板显示正确
            GD.Print($"后退: 力量={accelerationForce}, 方向={forceDirection}");

            isMoving = true;
        }

        // 更新速度标签
        if (SpeedLabel != null)
        {
            string speedText = $"速度: {currentSpeed:F1} px/s";

            // 如果处于加速状态，显示加速状态指示
            if (isBoosting)
            {
                speedText += " [加速]";
            }

            // 添加最大速度百分比
            float maxSpeedToUse = isBoosting ? ShipData.MaxSpeed * BOOST_SPEED_MULTIPLIER : ShipData.MaxSpeed;
            float speedPercent = (currentSpeed / maxSpeedToUse) * 100;
            speedText += $" ({speedPercent:F0}%)";

            SpeedLabel.Text = speedText;
        }

        // Q/E 控制左右旋转
        if (Input.IsKeyPressed(Key.Q))
        {
            float turningForce = ShipData.TurningAcceleration * 10;
            if (isShiftPressed && isMoving)
            {
                // 移动时按下Shift+Q，转向也加速
                turningForce *= BOOST_TURNING_MULTIPLIER;
            }
            torque -= turningForce;
        }
        if (Input.IsKeyPressed(Key.E))
        {
            float turningForce = ShipData.TurningAcceleration * 10;
            if (isShiftPressed && isMoving)
            {
                // 移动时按下Shift+E，转向也加速
                turningForce *= BOOST_TURNING_MULTIPLIER;
            }
            torque += turningForce;
        }

        // 根据移动和加速状态更新飞船状态
        if (isMoving)
        {
            if (isBoosting)
            {
                SetShipState(ShipState.Boosting); // 加速状态
            }
            else
            {
                SetShipState(ShipState.On); // 正常移动状态
            }
        }
        else
        {
            SetShipState(ShipState.Idle); // 无移动输入时引擎待机
        }

        ApplyCentralForce(force);
        ApplyTorque(torque);

        // 计算当前最大速度限制
        float currentMaxSpeed = ShipData.MaxSpeed;
        if (isBoosting)
        {
            currentMaxSpeed *= BOOST_SPEED_MULTIPLIER; // 加速状态下提高最大速度
        }

        // 限制最大速度
        if (LinearVelocity.Length() > currentMaxSpeed)
        {
            LinearVelocity = LinearVelocity.Normalized() * currentMaxSpeed;
            // 更新当前速度
            ShipData.CurrentSpeed = currentMaxSpeed;
        }

        // 计算当前最大转向速度限制
        float currentMaxTurningSpeed = ShipData.MaxTurningSpeed;
        if (isBoosting && isMoving)
        {
            currentMaxTurningSpeed *= BOOST_TURNING_MULTIPLIER; // 加速状态下提高最大转向速度
        }

        // 限制最大角速度
        if (Mathf.Abs(AngularVelocity) > currentMaxTurningSpeed)
        {
            AngularVelocity = Mathf.Sign(AngularVelocity) * currentMaxTurningSpeed;
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
        if (EngineMount == null) return;

        foreach (Node child in EngineMount.GetChildren())
        {
            if (child is Engine engine)
            {
                // 将飞船状态映射到引擎状态
                engine.SetEngineState((Engine.EngineState)currentState);
            }
        }
    }

    [Export]
    // 引擎槽位节点
    public Node EngineMount; // 引擎槽位节点
    [Export]
    // 武器槽位node
    public Node WeaponHardpoint; // 武器槽位节点
    public bool IsControlled = false; // 是否受控
}

// 武器槽位
public partial class WeaponHardpoint : Node2D
{
    // 类型分为（能量，导弹，动能）
    public enum WeaponType
    {
        Energy, // 能量武器
        Missile, // 导弹武器
        Kinetic // 动能武器
    }
    public WeaponType Type; // 武器类型
                            // 槽位大小（分为大中小特）
    public enum HardpointSize
    {
        Small, // 小型槽位
        Medium, // 中型槽位
        Large, // 大型槽位
        ExtraLarge // 特大型槽位
    }

}

