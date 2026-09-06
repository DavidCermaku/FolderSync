namespace FolderSync.Logger;

public record LogRecord
{
	public string Message { get; init; }
	public ELogLevel Level { get; init; }
	public DateTime Timestamp { get; init; }

	public LogRecord (string message, ELogLevel level, DateTime timestamp)
	{
		Message = message;
		Level = level;
		Timestamp = timestamp;
	}

	public override string ToString() =>
		$"{Timestamp.ToString()}: {Level.ToString()}\t {Message}";
}