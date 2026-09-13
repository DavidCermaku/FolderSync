using System.CommandLine;
using System.CommandLine.Parsing;

namespace FolderSync.CommandLine;

internal static class OptionsParser
{
	public static FolderSyncOptions Parse(string[] args)
	{
		var parsedArgsResult = CommandLineParser.Parse(CommandLineArguments.Command, args);
		EnsureArgsParsed(parsedArgsResult);

		var options = FolderSyncOptions.FromParseResult(parsedArgsResult);
		EnsureOptionsValid(options);

		return options;
	}

	private static void EnsureOptionsValid(FolderSyncOptions options)
	{
		var validationErrors = OptionsValidator.Validate(options);
		if (validationErrors.Count == 0) return;

		throw new ArgumentException(string.Join(Environment.NewLine, validationErrors));
	}

	private static void EnsureArgsParsed(ParseResult parsedArgsResult)
	{
		if (!parsedArgsResult.Errors.Any()) return;

		var messages = parsedArgsResult.Errors.Select(error => error.Message);

		throw new ArgumentException(string.Join(Environment.NewLine, messages));
	}
}
