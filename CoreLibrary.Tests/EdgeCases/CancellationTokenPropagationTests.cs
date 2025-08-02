using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.EdgeCases
{
    public class CancellationTokenPropagationTests
    {
        [Fact(Timeout = 2000)]
        public async Task StopToken_ShutsListener()
        {
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());

            var communicator = new TcpCommunicator(configuration);
            var cancellationTokenSource = new CancellationTokenSource();
            await communicator.StartAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            // Accept loop runs in background; Dispose shouldn't block.
            var disposeTask = Task.Run(() => communicator.DisposeAsync());
            await disposeTask.TimeoutAfter(TimeSpan.FromMilliseconds(500));
        }
    }
}