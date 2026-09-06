namespace FolderSync.Logger;

public static class LoggerExtensions
{
    public static void LogInfo(this ILogger logger, string message) => logger.Log(ELogLevel.Info, message);

    public static void LogWarning(this ILogger logger, string message) => logger.Log(ELogLevel.Warning, message);

    public static void LogError(this ILogger logger, string message) => logger.Log(ELogLevel.Error, message);

    public static void Log(this ILogger logger, ELogLevel level, string message) =>
        logger.Log(new LogRecord(message, level, DateTime.UtcNow));
}
