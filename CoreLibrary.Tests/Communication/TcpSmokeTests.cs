using CoreLibrary.Communication.TcpCommunication;      // TcpCommunicator
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Communication
{
    public class TcpSmokeTests
    {
        [Fact]
        public async Task Tcp_StartAndDispose_DoesNotThrow()
        {
            var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());

            using var comm = new TcpCommunicator(cfg);
            await comm.StartAsync();         // listener bound before Dispose
            await comm.Started;

            using var client = new TcpCommunicator(cfg);
            await client.SendMessageAsync(new Message("test", "ping"));
        }
    }
}
