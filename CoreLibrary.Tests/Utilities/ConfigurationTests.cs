using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Utilities
{
    public class ConfigurationTests
    {
        [Fact]
        public void IsEndpointBusy_ReturnsFalse_WhenPortFree()
        {
            var cfg = new Configuration { IpAddress = "127.0.0.1", Port = PortFinder.FreePort() };
            Assert.False(cfg.IsEndpointBusy());
        }

        [Fact]
        public void IsEndpointBusy_ReturnsTrue_WhenPortBound()
        {
            int port = PortFinder.FreePort();
            using var tcp = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
            tcp.Start();

            var cfg = new Configuration { IpAddress = "127.0.0.1", Port = port };
            Assert.True(cfg.IsEndpointBusy());
        }

        [Fact]
        public void ClientId_Unique_ByDefault()
        {
            var a = new Configuration();
            var b = new Configuration();
            Assert.NotEqual(a.ClientId, b.ClientId);
        }
    }
}