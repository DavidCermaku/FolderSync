using FolderSync.IO.FileCompare;
using FolderSync.Logger;

namespace FolderSync.IO;

internal static class FileHelpers
{
	public static void CopyDirectory(string sourceDir, string destinationDir, ILogger logger)
	{
		var dir = new DirectoryInfo(sourceDir);

		if (!dir.Exists)
			throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

		Directory.CreateDirectory(destinationDir);

		foreach (var file in dir.GetFiles())
		{
			var targetFilePath = Path.Combine(destinationDir, file.Name);

			if (File.Exists(targetFilePath))
			{
				var targetFile = new FileInfo(targetFilePath);
				if (FileComparer.CompareFiles(file, targetFile))
				{
					logger.LogInfo($"\nSKIPPING file: {file.FullName}\nSAME ALREADY EXISTS at: {targetFile.FullName}");
				}
				else
				{
					logger.LogInfo($"FILE ALREADY EXISTS BUT DIFFERENT, REPLACING OLD: {targetFile.FullName}");

					CopyFile(file, targetFilePath, logger);
				}
			}
			else
			{
				CopyFile(file, targetFilePath, logger);
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
			throw new DirectoryNotFoundException($"Replica directory not found: {dirReplica.FullName}");

		var dirSource = new DirectoryInfo(sourceDir);
		if (!dirSource.Exists)
		{
			dirReplica.Delete(true);
		}
		else
		{
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
	}

	private static void CopyFile(FileInfo file, string targetFilePath, ILogger logger)
	{
		try
		{
			file.CopyTo(targetFilePath);
			logger.LogInfo($"CREATED file {targetFilePath}");
		}
		catch (Exception ex)
		{
			logger.LogError(ex.Message);
		}
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
			logger.LogError(ex.Message);
		}
	}
}
