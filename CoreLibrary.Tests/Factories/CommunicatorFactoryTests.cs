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
            var configuration = new Configuration { Transport = TransportKind.Udp };
            var communicator = CommunicatorFactory.Create(configuration);
            Assert.IsType<UdpCommunicator>(communicator);
            communicator.Dispose();
        }

        [Fact]
        public void Create_Throws_OnUnsupportedTransport()
        {
            var configuration = new Configuration { Transport = (TransportKind)999 };
            Assert.Throws<NotSupportedException>(() => CommunicatorFactory.Create(configuration));
        }
    }
}
