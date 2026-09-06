using System.CommandLine;
using System.CommandLine.Parsing;

namespace FolderSync.CommandLine;

internal static class OptionsParser
{
	public static FolderSyncOptions Parse(string[] args)
	{
		var parsedArgsResult = CommandLineParser.Parse(CommandLineArguments.Command, args);
		EnsureArgsParsed(parsedArgsResult);

		return FolderSyncOptions.FromParseResult(parsedArgsResult);
	}

	private static void EnsureArgsParsed(ParseResult parsedArgsResult)
	{
		if (!parsedArgsResult.Errors.Any()) return;

		foreach (var error in parsedArgsResult.Errors)
		{
			Console.Error.WriteLine($"Error when parsing input argument {error.SymbolResult}error during parsing.");
		}

		throw new ArgumentException();
	}
}
