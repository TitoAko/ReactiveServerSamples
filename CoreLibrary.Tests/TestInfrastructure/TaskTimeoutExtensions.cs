namespace CoreLibrary.Tests.TestInfrastructure;

internal static class TaskTimeoutExtensions
{
    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var completed = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));
        if (completed != task) throw new TimeoutException($"Task did not finish in {timeout}");
        return await task; // propagate result/exception
    }
}
