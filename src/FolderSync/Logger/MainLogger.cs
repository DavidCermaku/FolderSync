namespace FolderSync.Logger;

internal class MainLogger : ILogger, IDisposable
{
	private readonly ILogger[] _loggerVariants;

	public MainLogger(params ILogger[] loggerVariants)
	{
		_loggerVariants = loggerVariants;
	}

	public void Log(LogRecord record)
	{
		foreach (var logger in _loggerVariants)
		{
			logger.Log(record);
		}
	}

	public void Dispose()
	{
		foreach (var logger in _loggerVariants)
		{
			if (logger is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
