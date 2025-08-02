using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.Utilities
{
    public class ConfigurationTests
    {
        [Fact]
        public void Endpoint_Composes_Correctly()
        {
            var cfg = TestConfig.TcpLoopback(5555);
            Assert.Equal("127.0.0.1:5555", cfg.Endpoint);
        }

        [Fact]
        public void With_Expression_Produces_Clone()
        {
            var cfg1 = TestConfig.TcpLoopback();
            var cfg2 = cfg1 with { Port = cfg1.Port + 1 };

            Assert.NotEqual(cfg1.Port, cfg2.Port);
            Assert.Equal(cfg1.BindAddress, cfg2.BindAddress);
        }
    }
}