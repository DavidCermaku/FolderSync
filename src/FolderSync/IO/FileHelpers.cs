using FolderSync.IO.FileCompare;
using FolderSync.Logger;

namespace FolderSync.IO;

internal static class FileHelpers
{
	public static void CopyDirectory(string sourceDir, string destinationDir, ILogger logger)
	{
		var dir = new DirectoryInfo(sourceDir);

		if (!dir.Exists)
			throw new DirectoryNotFoundException($"Source directory does not exist: {dir.FullName}");

		CreateDirectory(destinationDir, logger);

		foreach (var file in dir.GetFiles())
		{
			var targetFilePath = Path.Combine(destinationDir, file.Name);

			try
			{
				if (Directory.Exists(targetFilePath))
				{
					RemoveDirectory(new DirectoryInfo(targetFilePath), logger);
				}

				if (File.Exists(targetFilePath) && FileComparer.CompareFiles(file, new FileInfo(targetFilePath)))
				{
					continue;
				}

				CopyFile(file, targetFilePath, logger);
			}
			catch (Exception ex)
			{
				logger.LogError($"Failed to copy file {file.FullName}: {ex.Message}");
			}
		}

		var dirs = dir.GetDirectories();
		foreach (var subDir in dirs)
		{
			var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
			CopyDirectory(subDir.FullName, newDestinationDir, logger);
		}
	}

	internal static void RemoveReplicaDirFilesNotInSourceDir(string replicaDir, string sourceDir, ILogger logger)
	{
		var dirReplica = new DirectoryInfo(replicaDir);
		if (!dirReplica.Exists)
			throw new DirectoryNotFoundException($"Replica directory does not exist: {dirReplica.FullName}");

		var dirSource = new DirectoryInfo(sourceDir);
		if (!dirSource.Exists)
		{
			RemoveDirectory(dirReplica, logger);
			return;
		}

		var filesToRemove = dirReplica.GetFiles()
					.Where(file => !File.Exists(Path.Combine(sourceDir, file.Name)));

		foreach (var file in filesToRemove)
		{
			RemoveFile(file, logger);
		}

		var dirs = dirReplica.GetDirectories();
		foreach (var subDir in dirs)
		{
			var newReplicaDir = Path.Combine(replicaDir, subDir.Name);
			var newSourceDir = Path.Combine(sourceDir, subDir.Name);
			RemoveReplicaDirFilesNotInSourceDir(newReplicaDir, newSourceDir, logger);
		}
	}

	private static void CreateDirectory(string destinationDir, ILogger logger)
	{
		if (Directory.Exists(destinationDir)) return;

		if (File.Exists(destinationDir))
		{
			RemoveFile(new FileInfo(destinationDir), logger);
		}

		Directory.CreateDirectory(destinationDir);
		logger.LogInfo($"CREATED directory {destinationDir}");
	}

	private static void RemoveDirectory(DirectoryInfo dir, ILogger logger)
	{
		try
		{
			dir.Delete(true);
			logger.LogInfo($"REMOVED directory {dir.FullName}");
		}
		catch (Exception ex)
		{
			logger.LogError($"ERROR on removing directory {dir.FullName}: {ex.Message}");
		}
	}

	private static void CopyFile(FileInfo file, string targetFilePath, ILogger logger)
	{
		var replacing = File.Exists(targetFilePath);

		file.CopyTo(targetFilePath, true);

		logger.LogInfo(replacing
			? $"REPLACED file {targetFilePath}"
			: $"CREATED file {targetFilePath}");
	}

	private static void RemoveFile(FileInfo file, ILogger logger)
	{
		try
		{
			file.Delete();
			logger.LogInfo($"REMOVED file {file.FullName}");
		}
		catch (Exception ex)
		{
			logger.LogError($"ERROR during file removal: {file.FullName}:\n{ex.Message}");
		}
	}
}
