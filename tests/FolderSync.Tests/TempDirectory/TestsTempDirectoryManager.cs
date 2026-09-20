internal class TestsTempDirectoryManager : IDisposable
{
	public string SourceDirPath { get; }
	public string ReplicaDirPath { get; }

	public void WriteSourceFile(string relativePath, string content) =>
		File.WriteAllText(Path.Combine(SourceDirPath, relativePath), content);

	public void CreateSourceDirectory(string relativePath) =>
		Directory.CreateDirectory(Path.Combine(SourceDirPath, relativePath));

	public void RemoveSourceDirectory() => Directory.Delete(SourceDirPath, true);

	public void RecreateSourceDirectory() => Directory.CreateDirectory(SourceDirPath);

	public void WriteReplicaFile(string relativePath, string content) =>
		File.WriteAllText(Path.Combine(ReplicaDirPath, relativePath), content);

	public void CreateReplicaDirectory(string relativePath) =>
		Directory.CreateDirectory(Path.Combine(ReplicaDirPath, relativePath));

	public bool ReplicaHasFile(string relativePath) =>
		File.Exists(Path.Combine(ReplicaDirPath, relativePath));

	public bool ReplicaHasDirectory(string relativePath) =>
		Directory.Exists(Path.Combine(ReplicaDirPath, relativePath));

	public string ReadReplicaFile(string relativePath) =>
		File.ReadAllText(Path.Combine(ReplicaDirPath, relativePath));

	public bool ReplicaFileHasContent(string relativePath, string content)
	{
		if (!ReplicaHasFile(relativePath)) return false;

		try
		{
			return ReadReplicaFile(relativePath) == content;
		}
		catch
		{
			return false;
		}
	}

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
