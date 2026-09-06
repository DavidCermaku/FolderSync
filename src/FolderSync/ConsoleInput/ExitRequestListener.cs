namespace FolderSync.ConsoleInput;

internal static class ExitRequestListener
{
	public static Task WaitForEscapeAsync(CancellationTokenSource cts)
	{
		return Task.Run(() =>
		{
			while (!cts.IsCancellationRequested)
			{
				var keyInfo = Console.ReadKey(intercept: true);
				if (keyInfo.Key == ConsoleKey.Escape)
				{
					cts.Cancel();
				}
			}
		});
	}
}
