using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.EdgeCases
{

    public class TcpZeroLengthPayloadTests : IAsyncLifetime
    {
        private TcpCommunicator? _server;
        private TcpCommunicator? _client;

        public async Task InitializeAsync()
        {
            var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());
            _server = new TcpCommunicator(cfg);
            await _server.StartAsync();
            await _server.Started;
            _client = new TcpCommunicator(cfg);
        }

        [Fact]
        public async Task EmptyMessage_IsRejected()
        {
            var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());

            using var sender = new TcpSender(cfg);

            var empty = new Message("cli", "");

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sender.SendAsync(empty));
        }

        public Task DisposeAsync()
        {
            _server?.Dispose();
            _client?.Dispose();
            return Task.CompletedTask;
        }
    }
}