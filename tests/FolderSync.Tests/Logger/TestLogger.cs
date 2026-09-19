using FolderSync.Logger;

internal class TestLogger : ILogger
{
	public readonly List<LogRecord> LogReports = new();

	public void Log(LogRecord record) => LogReports.Add(record);
}
