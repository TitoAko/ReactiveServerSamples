using CoreLibrary.Utilities;
using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Tests.Utilities
{
    public class ConfigurationTests
    {
        private IConfigurationRoot CreateInMemoryConfig(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values!)
                .Build();
        }

        [Fact]
        public void ShouldLoadAllValuesFromConfig()
        {
            var values = new Dictionary<string, string?>
            {
                ["AppConfig:Username"] = "Alice",
                ["AppConfig:Password"] = "secret",
                ["AppConfig:IpAddress"] = "127.0.0.1",
                ["AppConfig:Port"] = "9000",
                ["AppConfig:Communicator"] = "UdpCommunicator",
                ["AppConfig:AppType"] = "Client"
            };

            var config = new Configuration(CreateInMemoryConfig(values));

            Assert.Equal("Alice", config.Username);
            Assert.Equal("secret", config.Password);
            Assert.Equal("127.0.0.1", config.IpAddress);
            Assert.Equal(9000, config.Port);
            Assert.Equal("UdpCommunicator", config.Communicator);
            Assert.Equal("Client", config.AppType);
        }

        [Fact]
        public void ShouldThrowIfAppTypeIsMissing()
        {
            var values = new Dictionary<string, string?>
            {
                ["AppConfig:Username"] = "Bob",
                ["AppConfig:Password"] = "pass",
                ["AppConfig:IpAddress"] = "localhost",
                ["AppConfig:Port"] = "9001",
                ["AppConfig:Communicator"] = "TcpCommunicator"
                // AppType is missing!
            };

            var configData = CreateInMemoryConfig(values);

            Assert.Throws<InvalidOperationException>(() => new Configuration(configData));
        }
    }
}
