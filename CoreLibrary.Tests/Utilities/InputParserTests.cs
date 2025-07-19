using ClientApp;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using Microsoft.Extensions.Configuration;

namespace CoreLibrary.Tests.Utilities
{
    public class InputHandlerTests
    {
        private readonly Configuration _config;

        public InputHandlerTests()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppConfig:Username"] = "TestUser",
                    ["AppConfig:Password"] = "pass",
                    ["AppConfig:IpAddress"] = "127.0.0.1",
                    ["AppConfig:Port"] = "12345",
                    ["AppConfig:Communicator"] = "UdpCommunicator",
                    ["AppConfig:AppType"] = "Client"
                });

            _config = new Configuration(builder.Build());
        }

        [Theory]
        [InlineData("exit")]
        [InlineData("EXIT")]
        [InlineData("   exit")]
        [InlineData("exit   ")]
        [InlineData("  exit  ")]
        [InlineData("\nexit\n")]
        public void ShouldReturnExitFlagWhenInputIsExit(string input)
        {
            var (shouldExit, message) = InputHandler.Parse(input, _config.Username);

            Assert.True(shouldExit);
            Assert.Null(message);
        }

        [Theory]
        [InlineData("exit now")]
        [InlineData("please exit")]
        [InlineData("exit the room")]
        [InlineData(" exiting")]
        public void ShouldNotExitForEmbeddedExit(string input)
        {
            var (shouldExit, message) = InputHandler.Parse(input, _config.Username);

            Assert.False(shouldExit);
            Assert.NotNull(message);
            Assert.Equal(input, message!.Content);
        }

        [Theory]
        [InlineData("Hello world!")]
        [InlineData("This is a valid message.")]
        [InlineData("     lots of spaces    ")]
        [InlineData("\tTabbed input")]
        public void ShouldReturnMessageForValidInput(string input)
        {
            var (shouldExit, message) = InputHandler.Parse(input, _config.Username);

            Assert.False(shouldExit);
            Assert.NotNull(message);
            Assert.Equal(_config.Username, message!.Sender);
            Assert.Equal(input, message.Content);
            Assert.IsType<TextMessage>(message.MessageType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldIgnoreEmptyInput(string input)
        {
            var (shouldExit, message) = InputHandler.Parse(input, _config.Username);

            Assert.False(shouldExit);
            Assert.Null(message);
        }

        [Fact]
        public void ShouldHandleUnexpectedInputGracefully()
        {
            var input = new string('x', 10_000); // simulate large/gibberish input
            var (shouldExit, message) = InputHandler.Parse(input, _config.Username);

            Assert.False(shouldExit);
            Assert.NotNull(message);
            Assert.Equal(input, message!.Content);
        }
    }
}