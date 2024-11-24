namespace TikiShop.Core.Services.LogService;

public class LogService<T> : ILogService<T> where T : class
{
    private readonly ILogger<T> _logger;

    public LogService(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogInformation($"[{timestamp}] {message}");
    }

    public void LogWarning(string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogWarning($"[{timestamp}] {message}");
    }

    public void LogError(Exception exception, string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogError(exception, $"[{timestamp}] {message}");
    }

    public void LogError(string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogError($"[{timestamp}] {message}");
    }

    public void LogTrace(string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogTrace($"[{timestamp}] {message}");
    }

    public void LogCritical(string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _logger.LogCritical($"[{timestamp}] {message}");
    }
}