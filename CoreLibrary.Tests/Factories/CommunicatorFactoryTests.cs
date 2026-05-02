using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Factories;
using CoreLibrary.Interfaces;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Factories
{
    public class CommunicatorFactoryTests
    {
        [Theory]
        [InlineData("UdpCommunicator", typeof(UdpCommunicator))]
        [InlineData("TcpCommunicator", typeof(TcpCommunicator))]
        public void Factory_Resolves_By_String(string communicator, Type expectedConcrete)
        {
            var cfg = TestConfig.UdpLoopback() with { Communicator = communicator };

            ICommunicator instance = CommunicatorFactory.Create(cfg);

            Assert.IsType(expectedConcrete, instance);
        }

        [Fact]
        public void Factory_Throws_On_Unknown_Communicator()
        {
            var cfg = TestConfig.UdpLoopback() with { Communicator = "Unknown" };

            Assert.Throws<ArgumentException>(() =>
                CommunicatorFactory.Create(cfg));
        }

        [Theory]
        [InlineData("udpcommunicator", typeof(UdpCommunicator))]
        [InlineData("TCPCommunicator", typeof(TcpCommunicator))]
        public void Factory_Is_Case_Insensitive(string communicator, Type expectedConcrete)
        {
            var cfg = TestConfig.UdpLoopback() with { Communicator = communicator };

            var instance = CommunicatorFactory.Create(cfg);

            Assert.IsType(expectedConcrete, instance);
        }
    }
}
