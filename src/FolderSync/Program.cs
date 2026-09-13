using FolderSync.CommandLine;
using FolderSync.ConsoleInput;
using FolderSync.Logger;
using FolderSync.Services;

namespace FolderSync;

public static class Program
{
	static async Task<int> Main(string[] args)
	{
		FolderSyncOptions options;
		try
		{
			options = OptionsParser.Parse(args);
		}
		catch (ArgumentException ex)
		{
			Console.Error.WriteLine(ex.Message);
			return 1;
		}

		using var logger = LoggerFactory.Create(options.LogFilePath);
		var folderSyncService = new FolderSynchornizationService(logger, options.SourcePath, options.ReplicaPath, options.Interval);

		using var cts = new CancellationTokenSource();
		var folderSyncTask = folderSyncService.StartAsync(cts.Token);

		await ExitRequestListener.WaitForEscapeAsync(cts);
		await folderSyncTask;

		return 0;
	}
}
