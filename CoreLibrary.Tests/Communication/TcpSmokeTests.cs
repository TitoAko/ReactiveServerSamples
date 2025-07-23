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
            var cfg = new Configuration
            {
                IpAddress = "127.0.0.1",
                Port = PortFinder.FreePort(),
                Transport = TransportKind.Tcp
            };

            using var comm = new TcpCommunicator(cfg);
            _ = comm.StartAsync();               // fire-and-forget, no await needed
            await Task.Delay(50);            // give listener a moment

            await comm.SendMessageAsync(new Message("test", "ping"));
        }
    }
}
