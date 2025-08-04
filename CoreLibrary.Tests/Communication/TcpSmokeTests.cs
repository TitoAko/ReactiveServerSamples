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
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());

            await using var communicator = new TcpCommunicator(configuration);
            await communicator.StartAsync();         // listener bound before Dispose

            await using var client = new TcpCommunicator(configuration);
            await client.SendMessageAsync(new Message("test", "ping"));
        }
    }
}
