namespace TikiShop.Core.Services.LogService;

public interface ILogService<T> where T : class
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(Exception exception, string message);
    void LogError(string message);
    void LogTrace(string message);
    void LogCritical(string message);
}