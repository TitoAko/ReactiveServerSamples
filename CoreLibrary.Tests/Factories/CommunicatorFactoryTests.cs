using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Factories;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Factories
{
    public class CommunicatorFactoryTests
    {
        [Fact]
        public void Create_ReturnsUdp_WhenConfigured()
        {
            var cfg = new Configuration { Transport = TransportKind.Udp };
            var comm = CommunicatorFactory.Create(cfg);
            Assert.IsType<UdpCommunicator>(comm);
            comm.Dispose();
        }

        [Fact]
        public void Create_Throws_OnUnsupportedTransport()
        {
            var cfg = new Configuration { Transport = (TransportKind)999 };
            Assert.Throws<NotSupportedException>(() => CommunicatorFactory.Create(cfg));
        }
    }
}
