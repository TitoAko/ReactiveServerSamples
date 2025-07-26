using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication
{
    public class TcpIntegrationTests : IAsyncLifetime
    {
        private readonly int _port = PortFinder.FreePort();
        private readonly Configuration _configuration;
        private readonly TcpCommunicator _server;
        private TcpCommunicator? _client;

        private readonly List<Message> _received = new();

        public TcpIntegrationTests()
        {
            _configuration = TestConfig.TcpLoopback(PortFinder.FreePort());
            _server = new TcpCommunicator(_configuration);
            _server.MessageReceived += (_, m) => _received.Add(m);
        }

        public async Task InitializeAsync()
        {
            await _server.StartAsync();
            await _server.Started;
            _client = new TcpCommunicator(_configuration); // reinitialize client after server started
        }
        public Task DisposeAsync()
        {
            _server.Dispose();
            _client?.Dispose();
            return Task.CompletedTask;
        }

        [Fact(Timeout = 2000)]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var message = new Message("cli", "hello");
            await _client!.SendMessageAsync(message).TimeoutAfter(TimeSpan.FromSeconds(1));
            await TaskTimeoutExtensions.WaitForMessageAsync<Message>(_received, 1, 1000);
            _received.Single().Content.Should().Be("hello");
        }
    }
}
