namespace FolderSync.Logger;

public class FileLogger : ILogger, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly Lock _lock = new();

    public FileLogger(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        _writer = new StreamWriter(stream) { AutoFlush = true };
    }

    public void Log(LogRecord record)
    {
        var recordText = record.ToString();
        lock (_lock)
        {
            _writer.WriteLine(recordText);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _writer.Dispose();
        }
    }
}
