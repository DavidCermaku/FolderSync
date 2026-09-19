internal class TestsTempDirectoryManager : IDisposable
{
	public string SourceDirPath { get; }
	public string ReplicaDirPath { get; }

	public void WriteSourceFile(string relativePath, string content) =>
		File.WriteAllText(Path.Combine(SourceDirPath, relativePath), content);

	public bool ReplicaHasFile(string relativePath) =>
		File.Exists(Path.Combine(ReplicaDirPath, relativePath));

	public string ReadReplicaFile(string relativePath) =>
		File.ReadAllText(Path.Combine(ReplicaDirPath, relativePath));

	private readonly string _rootPath;

	public TestsTempDirectoryManager()
	{
		_rootPath = Path.Combine(Path.GetTempPath(), "TestsTempDirectory", Guid.NewGuid().ToString());
		SourceDirPath = Path.Combine(_rootPath, "source");
		ReplicaDirPath = Path.Combine(_rootPath, "replica");

		Directory.CreateDirectory(SourceDirPath);
		Directory.CreateDirectory(ReplicaDirPath);
	}

	public void Dispose()
	{
		try
		{
			Directory.Delete(_rootPath, true);
		}
		catch
		{
		}
	}
}
