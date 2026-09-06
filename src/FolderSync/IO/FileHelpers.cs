using FolderSync.IO.FileCompare;
using FolderSync.Logger;

namespace FolderSync.IO;

internal static class FileHelpers
{
	public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, ILogger logger)
	{
		var dir = new DirectoryInfo(sourceDir);

		if (!dir.Exists)
			throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

		DirectoryInfo[] dirs = dir.GetDirectories();

		Directory.CreateDirectory(destinationDir);

		foreach (FileInfo file in dir.GetFiles())
		{
			string targetFilePath = Path.Combine(destinationDir, file.Name);
			
			if (File.Exists(targetFilePath))
			{
				var targetFile = new FileInfo(targetFilePath);
				if (FileComparer.CompareFiles(file, targetFile))
				{
					logger.LogInfo($"\nSKIPPING file: {file.FullName}\nSAME ALREADY EXISTS at: {targetFile.FullName}");
				}
				else
				{
					CopyFile(file, targetFilePath, logger);
				}
			}
			else
			{
				CopyFile(file, targetFilePath, logger);
			}
		}

		if (recursive)
		{
			foreach (DirectoryInfo subDir in dirs)
			{
				string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
				CopyDirectory(subDir.FullName, newDestinationDir, true, logger);
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
}
