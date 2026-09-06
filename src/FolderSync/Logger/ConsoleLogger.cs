namespace FolderSync.Logger;

public class ConsoleLogger : ILogger
{
    private readonly TextWriter _writer;
    private readonly Lock _lock = new();

    public ConsoleLogger(TextWriter writer)
    {
        _writer = writer;
    }

    public void Log(LogRecord record)
    {
        var recordText = record.ToString();
        lock (_lock)
        {
            _writer.WriteLine(recordText);
        }
    }
}
