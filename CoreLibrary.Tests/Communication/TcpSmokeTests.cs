using CoreLibrary.Communication.TcpCommunication;      // TcpCommunicator
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Communication
{

    public class TcpSmokeTests
    {
        [Fact]
        public async Task Tcp_StartAndDispose_DoesNotThrow()
        {
            var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());

            using var server = new TcpCommunicator(cfg);
            await server.StartAsync();         // listener bound before Dispose
            
            using var client = new TcpCommunicator(cfg);
            await Task.Delay(50);            // give listener a moment
            await client.SendMessageAsync(new Message("test", "ping"));
        }
    }
}
