using System.CommandLine;

namespace FolderSync.CommandLine;


public record FolderSyncOptions(
	string SourcePath,
	string ReplicaPath,
	TimeSpan Interval,
	string LogFilePath)
{
	public static FolderSyncOptions FromParseResult(ParseResult parsed) => new(
		SourcePath: parsed.GetValue(CommandLineArguments.SourceArg)!,
		ReplicaPath: parsed.GetValue(CommandLineArguments.ReplicaArg)!,
		Interval: TimeSpan.FromSeconds(parsed.GetValue(CommandLineArguments.IntervalArg)!),
		LogFilePath: parsed.GetValue(CommandLineArguments.LogFileArg)!);
}
