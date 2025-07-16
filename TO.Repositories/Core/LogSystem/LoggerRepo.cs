using Godot;
using TO.Commons.Enums.System;
using TO.Repositories.Abstractions.Core.LogSystem;
using FileAccess = Godot.FileAccess;

namespace TO.Repositories.Core.LogSystem;

public class LoggerRepo : ILoggerRepo
{
    // 默认日志颜色配置
    private static readonly Dictionary<LogLevel, Color> DefaultLogColors = new()
    {
        { LogLevel.Debug, Colors.Cyan },
        { LogLevel.Info, Colors.White },
        { LogLevel.Warning, Colors.Yellow },
        { LogLevel.Error, Colors.Red },
        { LogLevel.Fatal, Colors.DarkRed }
    };

    // 当前日志级别
    private LogLevel _currentLevel = LogLevel.Debug;
    // 是否启用文件日志
    private bool _enableFileLogging = false;
    // 日志文件路径
    private string _logFilePath = "user://logs/game.log";
    // 日志文件对象
    private FileAccess? _logFile;
    // 当前日志颜色配置
    private Dictionary<LogLevel, Color> _logColors = new(DefaultLogColors);

    /// <summary>
    /// 构造函数
    /// </summary>
    public LoggerRepo()
    {
        SetupFileLogging();
    }

    /// <summary>
    /// 设置日志级别
    /// </summary>
    /// <param name="level">日志级别</param>
    public void SetLevel(LogLevel level)
    {
        _currentLevel = level;
    }

    /// <summary>
    /// 设置指定级别的日志颜色
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="color">颜色</param>
    public void SetColor(LogLevel level, Color color)
    {
        _logColors[level] = color;
    }

    /// <summary>
    /// 设置多个日志级别的颜色
    /// </summary>
    /// <param name="colors">颜色配置字典</param>
    public void SetColors(Dictionary<LogLevel, Color> colors)
    {
        foreach (var kvp in colors)
        {
            if (_logColors.ContainsKey(kvp.Key))
            {
                _logColors[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// 重置所有日志颜色为默认值
    /// </summary>
    public void ResetColors()
    {
        _logColors = new Dictionary<LogLevel, Color>(DefaultLogColors);
    }

    /// <summary>
    /// 获取当前日志颜色配置
    /// </summary>
    /// <returns>颜色配置字典</returns>
    public Dictionary<LogLevel, Color> GetColors()
    {
        return new Dictionary<LogLevel, Color>(_logColors);
    }

    /// <summary>
    /// 设置是否启用文件日志
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void EnableFileLogging(bool enable)
    {
        _enableFileLogging = enable;
        if (enable)
        {
            SetupFileLogging();
        }
        else
        {
            _logFile?.Close();
            _logFile = null;
        }
    }

    /// <summary>
    /// 记录调试日志
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void Debug(string message, Dictionary<string, object>? context = null)
    {
        Log(LogLevel.Debug, message, context);
    }

    /// <summary>
    /// 记录信息日志
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void Info(string message, Dictionary<string, object>? context = null)
    {
        Log(LogLevel.Info, message, context);
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void Warning(string message, Dictionary<string, object>? context = null)
    {
        Log(LogLevel.Warning, message, context);
        GD.PushWarning(message);
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void Error(string message, Dictionary<string, object>? context = null)
    {
        Log(LogLevel.Error, message, context);
        PrintStack();
        GD.PushError(message);
    }

    /// <summary>
    /// 记录致命错误日志
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void Fatal(string message, Dictionary<string, object>? context = null)
    {
        Log(LogLevel.Fatal, message, context);
        PrintStack();
        GD.PushError(message);
    }

    /// <summary>
    /// 内部日志记录方法
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    private void Log(LogLevel level, string message, Dictionary<string, object>? context)
    {
        if (level < _currentLevel)
        {
            return;
        }

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var levelName = Enum.GetName(typeof(LogLevel), level);
        var formattedMessage = $"[{timestamp}] [{levelName}] {message}";

        if (context is { Count: > 0 })
        {
            formattedMessage += " | Context: " + System.Text.Json.JsonSerializer.Serialize(context);
        }

        // 控制台输出（带颜色）
        if (_logColors.TryGetValue(level, out Color color))
        {
            GD.PrintRich($"[color=#{color.ToHtml()}]{formattedMessage}[/color]");
        }
        else
        {
            GD.Print(formattedMessage);
        }

        // 文件日志
        if (_enableFileLogging)
        {
            _logFile?.StoreLine(formattedMessage);
        }
    }

    /// <summary>
    /// 打印调用堆栈
    /// </summary>
    private void PrintStack()
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var stackMessage = stackTrace.GetFrames().Aggregate("\nCall Stack:", (current, frame) => current + $"\n  at {frame.GetFileName()}:{frame.GetFileLineNumber()} - {frame.GetMethod().Name}()");

        if (_logColors.TryGetValue(LogLevel.Error, out Color color))
        {
            GD.PrintRich($"[color=#{color.ToHtml()}]{stackMessage}[/color]");
        }
    }

    /// <summary>
    /// 设置文件日志
    /// </summary>
    private void SetupFileLogging()
    {
        if (!_enableFileLogging)
        {
            return;
        }

        using var dir = DirAccess.Open("user://");
        if (dir != null && !dir.DirExists("logs"))
        {
            dir.MakeDir("logs");
        }

        _logFile = FileAccess.Open(_logFilePath, FileAccess.ModeFlags.Write);
    }
}
