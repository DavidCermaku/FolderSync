namespace FolderSync.Logger;

internal static class LoggerFactory
{
	public static MainLogger Create(string logFilePath)
	{
		var fileLogger = new FileLogger(logFilePath);
		var consoleLogger = new ConsoleLogger(Console.Out);
		return new MainLogger(consoleLogger, fileLogger);
	}
}
