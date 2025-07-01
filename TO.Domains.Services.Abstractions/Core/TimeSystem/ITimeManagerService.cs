using Infras.Writers;
using TO.Commons.Enums;

namespace TO.Domains.Services.Abstractions.Core.TimeSystem;

public interface ITimeManagerService
{
    /// <summary>
    /// 启动时间系统
    /// </summary>
    /// <param name="startTime">可选起始时间</param>
    void StartTimeSystem(GameTime startTime = default);

    /// <summary>
    /// 停止时间系统
    /// </summary>
    void StopTimeSystem();

    /// <summary>
    /// 获取格式化时间字符串
    /// </summary>
    string GetFormattedTime(string format = "yyyy-MM-dd HH:mm");

    /// <summary>
    /// 检查是否到达特定季节
    /// </summary>
    bool IsSeason(Season season);

    /// <summary>
    /// 获取当前季节
    /// </summary>
    Season GetCurrentSeason();

    /// <summary>
    /// 加速时间流逝
    /// </summary>
    /// <param name="factor">加速倍数</param>
    void AccelerateTime(float factor);

    /// <summary>
    /// 重置时间为正常速度
    /// </summary>
    void ResetTimeSpeed();
}