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
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());
            _server = new TcpCommunicator(configuration);
            await _server.StartAsync();
            //await _server.Started;
            _client = new TcpCommunicator(configuration);
        }

        [Fact]
        public async Task EmptyMessage_IsRejected()
        {
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());

            var sender = new TcpSender(configuration);

            var emptyMessage = new Message("cli", "");

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sender.SendAsync(emptyMessage));
        }

        public Task DisposeAsync()
        {
            _server?.DisposeAsync();
            _client?.DisposeAsync();
            return Task.CompletedTask;
        }
    }
}