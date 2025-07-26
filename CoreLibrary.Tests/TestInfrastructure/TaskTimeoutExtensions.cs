namespace CoreLibrary.Tests.TestInfrastructure;

internal static class TaskTimeoutExtensions
{
    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        var completed = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cancellationTokenSource.Token));
        if (completed != task)
        {
            throw new TimeoutException($"Task did not finish in {timeout}");
        }

        return await task; // propagate result/exception
    }
    public static async Task WaitForMessageAsync<T>(
        ICollection<T> collection, int expected, int timeoutMs = 1000)
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        while (stopWatch.ElapsedMilliseconds < timeoutMs)
        {
            if (collection.Count >= expected)
            {
                return;
            }

            await Task.Delay(25);
        }
        throw new TimeoutException($"Expected {expected} items but got {collection.Count}.");
    }
}
