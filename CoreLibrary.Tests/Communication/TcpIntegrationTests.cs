using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication
{
    public class TcpIntegrationTests : IAsyncLifetime
    {
        private readonly int _serverPort = PortFinder.FreePort();
        private readonly int _clientPort = PortFinder.FreePort();
        private readonly TcpCommunicator _server;
        private TcpCommunicator? _client;

        private readonly List<Message> _received = new();

        public TcpIntegrationTests()
        {
            var serverConfiguration = TestConfig.TcpLoopback(_serverPort);
            var clientConfiguration = TestConfig.TcpLoopback(_clientPort);
            _server = new TcpCommunicator(serverConfiguration);
            _client = new TcpCommunicator(clientConfiguration);
            _server.MessageReceived += (_, m) => _received.Add(m);
            _client!.MessageReceived += (_, m) => _received.Add(m);   // ⬅️ capture echoes
        }

        public async Task InitializeAsync()
        {
            _ = _server.StartAsync();
            _ = _client!.StartAsync();   // fire-and-forget, no await

            _server!.MessageReceived += async (_, m) =>
                await _server.SendMessageAsync(m);   // echo straight back
                                                     // after you create _client and start the listeners

        }
        public async Task DisposeAsync()
        {
            // If the fields might be null, null-check first
            if (_server is not null)
            {
                await _server.DisposeAsync();
            }

            if (_client is not null)
            {
                await _client.DisposeAsync();
            }
        }

        [Fact(Timeout = 2000)]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var message = new Message("cli", "hello");
            await _client!.SendMessageAsync(message);
            await TaskTimeoutExtensions.WaitForMessageAsync<Message>(_received, 1, 1000);
            _received.Single().Content.Should().Be("hello");
        }
    }
}
