namespace FolderSync.CommandLine;

public static class OptionsValidator
{
	public static IReadOnlyList<string> Validate(FolderSyncOptions options)
	{
		var validationErrorMessages = new List<string>();

		if (options.Interval <= TimeSpan.Zero)
		{
			validationErrorMessages.Add($"Interval is not greater than zero");
		}

		var source = string.Empty;
		var replica = string.Empty;
		var logFile = string.Empty;

		try
		{
			source = Normalize(options.SourcePath);
			replica = Normalize(options.ReplicaPath);
			logFile = Normalize(options.LogFilePath);
		}
		catch (Exception exception)
		{
			validationErrorMessages.Add($"Error during validation of paths: {exception.Message}");
			return validationErrorMessages;
		}

		if (AreSame(source, replica))
		{
			validationErrorMessages.Add($"Source and replica have same path");
		}
		else if (IsInside(replica, source))
		{
			validationErrorMessages.Add($"Replica path is in the source path");
		}
		else if (IsInside(source, replica))
		{
			validationErrorMessages.Add($"Source path is in replica path");
		}

		return validationErrorMessages;
	}

	private static string Normalize(string path) => Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));

	private static bool AreSame(string first, string second) => string.Equals(first, second, PathComparison);

	private static bool IsInside(string candidate, string root)
	{
		var rootPrefix = WithTrailingSeparator(root);
		var candidatePath = WithTrailingSeparator(candidate);

		return candidatePath.Length > rootPrefix.Length && candidatePath.StartsWith(rootPrefix, PathComparison);
	}

	private static string WithTrailingSeparator(string path) =>
		path.EndsWith(Path.DirectorySeparatorChar) ? path : path + Path.DirectorySeparatorChar;

	private const StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
}
