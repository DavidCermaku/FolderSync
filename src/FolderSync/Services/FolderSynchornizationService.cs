using System.Diagnostics;
using FolderSync.IO;
using FolderSync.Logger;

namespace FolderSync.Services;

public class FolderSynchornizationService
{
	private readonly ILogger _logger;
	private readonly string _sourceRootPath;
	private readonly string _replicaRootPath;
	private readonly TimeSpan _interval;
	private double _syncCycle;

	public FolderSynchornizationService(ILogger logger, string sourcePath, string replicaPath, TimeSpan interval)
	{
		_logger = logger;
		_sourceRootPath = Path.GetFullPath(sourcePath);
		_replicaRootPath = Path.GetFullPath(replicaPath);
		_interval = interval;
		_syncCycle = 0;
	}

	public async Task StartAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			var stopwatch = Stopwatch.StartNew();
			Synchronize();
			var elapsed = stopwatch.Elapsed;
			if (elapsed < _interval)
			{
				try
				{
					await Task.Delay(_interval - elapsed, ct);
				}
				catch (OperationCanceledException)
				{
					break;
				}
			}
		}
		_logger.LogInfo($"Stopping {nameof(FolderSynchornizationService)}");
	}

	private void Synchronize()
	{
		try
		{
			_syncCycle++;
			_logger.LogInfo($"Starting synchronization cycle {_syncCycle}");
			FileHelpers.CopyDirectory(_sourceRootPath, _replicaRootPath, _logger);
			FileHelpers.RemoveReplicaDirFilesNotInSourceDir(_replicaRootPath, _sourceRootPath, _logger);
			_logger.LogInfo($"Synchronization cycle {_syncCycle} finished");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
		}
	}
}
