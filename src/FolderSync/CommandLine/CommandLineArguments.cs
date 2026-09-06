using System.CommandLine;

namespace FolderSync.CommandLine;

internal static  class CommandLineArguments
{
	public static readonly Argument<string> SourceArg = new("source");
	public static readonly Argument<string> ReplicaArg = new("replica");
	public static readonly Argument<int> IntervalArg = new("interval-seconds");
	public static readonly Argument<string> LogFileArg = new("log-file");

	public static readonly RootCommand Command = new("FolderSync")
	{
		SourceArg, ReplicaArg, IntervalArg, LogFileArg
	};
}