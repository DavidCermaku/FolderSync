using FolderSync.Services;

namespace FolderSync.Tests;

public class FolderSynchornizationServiceTests : IDisposable
{
	public void Dispose() => _fixture.Dispose();

	private readonly SyncTestFixture _fixture = new();

	[Fact]
	public async Task StartAsync_CopiesSourceFileToReplica()
	{
		const string fileName = "testFile.txt";
		const string fileContent = "testText";

		_fixture.Directories.WriteSourceFile(fileName, fileContent);

		var service = _fixture.CreateService();

		using var cts = new CancellationTokenSource();
		var run = service.StartAsync(cts.Token);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(fileName),
			SyncTestFixture.WatchdogTimeSec);

		await cts.CancelAsync();
		await run;

		Assert.Equal(fileContent, _fixture.Directories.ReadReplicaFile(fileName));
	}
}

internal class SyncTestFixture : IDisposable
{
	public static readonly TimeSpan SyncIntervalMs = TimeSpan.FromMilliseconds(50);
	public static readonly TimeSpan WatchdogTimeSec = TimeSpan.FromSeconds(10);
	public TestsTempDirectoryManager Directories { get; } = new();
	public TestLogger Logger { get; } = new();
	public FolderSynchornizationService CreateService() =>
		new(Logger, Directories.SourceDirPath, Directories.ReplicaDirPath, SyncIntervalMs);
	public void Dispose() => Directories.Dispose();
}
