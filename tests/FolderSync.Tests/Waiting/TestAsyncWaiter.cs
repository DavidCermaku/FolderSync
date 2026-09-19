using System.Diagnostics;

internal static class TestAsyncWaiter
{
	private static readonly TimeSpan EvaluateIntervalMs = TimeSpan.FromMilliseconds(20);

	public static async Task WaitUntil(Func<bool> condition, TimeSpan watchdog)
	{
		var stopwatch = Stopwatch.StartNew();

		while (stopwatch.Elapsed < watchdog)
		{
			if (condition()) return;

			await Task.Delay(EvaluateIntervalMs);
		}

		Assert.Fail($"Watchdog timed out");
	}
}
