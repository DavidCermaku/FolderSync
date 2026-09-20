using FolderSync.Logger;
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
		const string fileContent = "test text";

		_fixture.Directories.WriteSourceFile(fileName, fileContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(fileName),
			SyncTestFixture.WatchdogTimeSec);

		Assert.Equal(fileContent, _fixture.Directories.ReadReplicaFile(fileName));
	}

	[Fact]
	public async Task StartAsync_CopiesNestedDirectoryToReplica()
	{
		const string dirName = "NestedFolder";
		const string filePath = $"{dirName}/nestedFile.txt";
		const string fileContent = "test text";

		_fixture.Directories.CreateSourceDirectory(dirName);
		_fixture.Directories.WriteSourceFile(filePath, fileContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(filePath),
			SyncTestFixture.WatchdogTimeSec);

		Assert.True(_fixture.Directories.ReplicaHasDirectory(dirName));
		Assert.Equal(fileContent, _fixture.Directories.ReadReplicaFile(filePath));
	}

	[Fact]
	public async Task StartAsync_UpdatesReplicaFileContentChangedInSource()
	{
		const string fileName = "changingFile.txt";
		const string originalContent = "original text content";
		const string changedContent = "changed text content";

		_fixture.Directories.WriteSourceFile(fileName, originalContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaFileHasContent(fileName, originalContent),
			SyncTestFixture.WatchdogTimeSec);

		_fixture.Directories.WriteSourceFile(fileName, changedContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaFileHasContent(fileName, changedContent),
			SyncTestFixture.WatchdogTimeSec);

		Assert.Equal(changedContent, _fixture.Directories.ReadReplicaFile(fileName));
	}

	[Fact]
	public async Task StartAsync_RemovesReplicaFileNotPresentInSource()
	{
		const string fileName = "replicaOnlyFile.txt";

		_fixture.Directories.WriteReplicaFile(fileName, "test text");

		await TestAsyncWaiter.WaitUntil(
			() => !_fixture.Directories.ReplicaHasFile(fileName),
			SyncTestFixture.WatchdogTimeSec);

		Assert.False(_fixture.Directories.ReplicaHasFile(fileName));
	}

	[Fact]
	public async Task StartAsync_RemovesReplicaFolderWithContentNotPresentInSource()
	{
		const string dirName = "ReplicaOnlyFolder";
		const string filePath = $"{dirName}/replicaOnlyFile.txt";

		_fixture.Directories.CreateReplicaDirectory(dirName);
		_fixture.Directories.WriteReplicaFile(filePath, "test text");

		await TestAsyncWaiter.WaitUntil(
			() => !_fixture.Directories.ReplicaHasDirectory(dirName),
			SyncTestFixture.WatchdogTimeSec);

		Assert.False(_fixture.Directories.ReplicaHasDirectory(dirName));
		Assert.False(_fixture.Directories.ReplicaHasFile(filePath));
	}

	[Fact]
	public async Task StartAsync_SynchronizesMultipleTimes()
	{
		const string firstFileName = "firstFile.txt";
		const string firstFileContent = "first test text";
		const string secondFileName = "secondFile.txt";
		const string secondFileContent = "second test text";

		_fixture.Directories.WriteSourceFile(firstFileName, firstFileContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(firstFileName),
			SyncTestFixture.WatchdogTimeSec);

		_fixture.Directories.WriteSourceFile(secondFileName, secondFileContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(secondFileName),
			SyncTestFixture.WatchdogTimeSec);

		Assert.Equal(firstFileContent, _fixture.Directories.ReadReplicaFile(firstFileName));
		Assert.Equal(secondFileContent, _fixture.Directories.ReadReplicaFile(secondFileName));
	}

	[Fact]
	public async Task StartAsync_RecoverFromRemovedSourceDirectory()
	{
		const string fileName = "recoveredFile.txt";
		const string fileContent = "recovered test text";

		_fixture.Directories.RemoveSourceDirectory();

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Logger.LogReports.Any(record => record.Level == ELogLevel.Error),
			SyncTestFixture.WatchdogTimeSec);

		_fixture.Directories.RecreateSourceDirectory();
		_fixture.Directories.WriteSourceFile(fileName, fileContent);

		await TestAsyncWaiter.WaitUntil(
			() => _fixture.Directories.ReplicaHasFile(fileName),
			SyncTestFixture.WatchdogTimeSec);

		Assert.Equal(fileContent, _fixture.Directories.ReadReplicaFile(fileName));
	}

	[Fact]
	public void StartAsync_StopsOnCancellationRequesteInitiated()
	{
		_fixture.StopService();

		Assert.Contains(
			_fixture.Logger.LogReports,
			record => record.Message == $"Stopping {nameof(FolderSynchornizationService)}");
	}
}

internal class SyncTestFixture : IDisposable
{
	public static readonly TimeSpan SyncIntervalMs = TimeSpan.FromMilliseconds(50);
	public static readonly TimeSpan WatchdogTimeSec = TimeSpan.FromSeconds(10);
	public TestsTempDirectoryManager Directories { get; } = new();
	public TestLogger Logger { get; } = new();

	private readonly FolderSynchornizationService _service;
	private readonly CancellationTokenSource _cts = new();
	private readonly Task _serviceTask;

	public SyncTestFixture()
	{
		_service = new FolderSynchornizationService(Logger, Directories.SourceDirPath, Directories.ReplicaDirPath, SyncIntervalMs);
		_serviceTask = _service.StartAsync(_cts.Token);
	}

	public void StopService()
	{
		_cts.Cancel();

		if (!_serviceTask.Wait(WatchdogTimeSec))
		{
			Assert.Fail($"{nameof(FolderSynchornizationService)} did not stop within the watchdog time");
		}
	}

	public void Dispose()
	{
		StopService();

		_cts.Dispose();
		Directories.Dispose();
	}
}
