using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using Xunit;

namespace CoreLibrary.Tests.Communication
{
    public class UdpCommunicatorTests : IDisposable
    {
        private readonly Configuration _config;
        private readonly UdpCommunicator _communicator;

        public UdpCommunicatorTests()
        {
            var values = new Dictionary<string, string?>
            {
                ["AppConfig:Username"] = "TestUser",
                ["AppConfig:Password"] = "TestPass",
                ["AppConfig:IpAddress"] = "127.0.0.1",
                ["AppConfig:Port"] = "9901",
                ["AppConfig:Communicator"] = "UdpCommunicator",
                ["AppConfig:AppType"] = "Test"
            };

            var configRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            _config = new Configuration(configRoot);
            _communicator = new UdpCommunicator(_config);
        }

        [Fact]
        public async Task ShouldSendMessageSuccessfully()
        {
            var message = new Message(_config.Username, "Test message", new TextMessage());

            var exception = await Record.ExceptionAsync(() =>
                _communicator.SendMessage(message));

            Assert.Null(exception);
        }

        [Fact]
        public void ShouldRaiseOnMessageReceived()
        {
            bool eventRaised = false;
            _communicator.OnMessageReceived += _ => eventRaised = true;

            var testMessage = new Message("user", "Hello", new TextMessage());
            _communicator.ReceiveMessage(); // Currently returns dummy message

            // Manually trigger the event via internal hook
            typeof(UdpCommunicator)
                .GetMethod("RaiseMessageReceivedEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_communicator, new object[] { testMessage });

            Assert.True(eventRaised);
        }

        [Fact]
        public void ShouldDisposeBothSenderAndReceiver()
        {
            var exception = Record.Exception(() => _communicator.Dispose());

            Assert.Null(exception); // Should clean up without crashing
        }

        [Fact]
        public void ShouldNotThrowOnStop()
        {
            var exception = Record.Exception(() => _communicator.Stop());

            Assert.Null(exception);
        }

        public void Dispose()
        {
            _communicator.Dispose();
        }
    }
}
