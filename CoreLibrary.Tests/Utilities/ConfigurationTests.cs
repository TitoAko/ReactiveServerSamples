using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Utilities
{
    public class ConfigurationTests
    {
        [Fact]
        public void Endpoint_Composes_Correctly_TCP()
        {
            var cfg = TestConfig.TcpLoopback(5555);
            Assert.Equal("127.0.0.1:5555", cfg.Endpoint);
        }

        [Fact]
        public void With_Expression_TCP_Produces_Clone()
        {
            var cfg1 = TestConfig.TcpLoopback();
            var cfg2 = cfg1 with { TargetPort = cfg1.TargetPort + 1 };

            Assert.NotSame(cfg1, cfg2);
            Assert.NotEqual(cfg1.TargetPort, cfg2.TargetPort);
            Assert.Equal(cfg1.ListenPort, cfg2.ListenPort);
            Assert.Equal(cfg1.BindAddress, cfg2.BindAddress);
            Assert.Equal(cfg1.TargetAddress, cfg2.TargetAddress);
        }

        [Fact]
        public void Endpoint_Composes_TargetAddress_And_TargetPort_UDP()
        {
            var cfg = TestConfig.UdpLoopback(listenPort: 1111, targetPort: 5555);

            Assert.Equal("127.0.0.1:5555", cfg.Endpoint);
        }

        [Fact]
        public void With_Expression_UDP_Produces_Clone()
        {
            var cfg1 = TestConfig.UdpLoopback(listenPort: 1000, targetPort: 2000);
            var cfg2 = cfg1 with { TargetPort = 3000 };

            Assert.NotEqual(cfg1.TargetPort, cfg2.TargetPort);
            Assert.Equal(cfg1.BindAddress, cfg2.BindAddress);
            Assert.Equal(cfg1.ListenPort, cfg2.ListenPort);
        }
    }
}