using FolderSync.Logger;

internal class TestLogger : ILogger
{
	public IReadOnlyList<LogRecord> LogReports
	{
		get
		{
			lock (_lock)
			{
				return _logReports.ToArray();
			}
		}
	}

	public void Log(LogRecord record)
	{
		lock (_lock)
		{
			_logReports.Add(record);
		}
	}

	private readonly List<LogRecord> _logReports = new();
	private readonly Lock _lock = new();
}
