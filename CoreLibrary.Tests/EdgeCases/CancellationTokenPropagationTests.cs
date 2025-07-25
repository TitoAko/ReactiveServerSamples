using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.EdgeCases
{
    public class CancellationTokenPropagationTests
    {
        [Fact(Timeout = 2000)]
        public async Task StopToken_ShutsListener()
        {
            var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());

            var comm = new TcpCommunicator(cfg);
            var cts = new CancellationTokenSource();
            await comm.StartAsync(cts.Token);
            cts.Cancel();

            // Accept loop runs in background; Dispose shouldn't block.
            var disposeTask = Task.Run(() => comm.Dispose());
            await disposeTask.TimeoutAfter(TimeSpan.FromMilliseconds(500));
        }
    }
}