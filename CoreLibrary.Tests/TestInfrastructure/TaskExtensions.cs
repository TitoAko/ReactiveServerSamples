namespace CoreLibrary.Tests.TestInfrastructure
{
    internal static class TaskExtensions
    {
        /// <summary>Await a task with a hard timeout.</summary>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            if (task != await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token)))
                throw new TimeoutException($"Task did not finish in {timeout}.");
            await task;                      // propagate exceptions
        }
    }
}
