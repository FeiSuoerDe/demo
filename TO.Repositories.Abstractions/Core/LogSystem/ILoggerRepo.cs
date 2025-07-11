namespace TO.Repositories.Abstractions.Core.LogSystem;

public interface ILoggerRepo
{
    void Debug(string message, Dictionary<string, object>? context = null);
    void Info(string message, Dictionary<string, object>? context = null);
    void Warning(string message, Dictionary<string, object>? context = null);
    void Error(string message, Dictionary<string, object>? context = null);
    void Fatal(string message, Dictionary<string, object>? context = null);
}