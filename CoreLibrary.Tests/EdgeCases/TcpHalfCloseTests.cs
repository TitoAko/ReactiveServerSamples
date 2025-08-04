using System.Net.Sockets;

using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Tests.TestInfrastructure;

namespace CoreLibrary.Tests.EdgeCases
{
    public class TcpHalfCloseTests : IAsyncLifetime
    {
        private TcpCommunicator? _server;
        private TcpClient? _rawClient;

        public async Task InitializeAsync()
        {
            var configuration = TestConfig.TcpLoopback(PortFinder.FreePort());
            _server = new TcpCommunicator(configuration);
            await _server.StartAsync();
            //await _server.Started;

            // raw client socket to simulate half-close
            _rawClient = new TcpClient();
            _rawClient.Connect("127.0.0.1", configuration.Port);
        }

        [Fact(Timeout = 2000)]
        public async Task ReadLoop_Exits_On_RemoteClose()
        {
            _rawClient?.Client.Shutdown(SocketShutdown.Send);   // half-close
            await Task.Delay(100);                       // give server time to read 0 bytes

            // dispose should not hang (listener loop must have exited)
            var tokenSource = new CancellationTokenSource(500);
            await Task.Run(() => _server!.DisposeAsync(), tokenSource.Token);
            Assert.False(tokenSource.IsCancellationRequested);
        }

        public Task DisposeAsync()
        {
            _rawClient?.Dispose();
            _server?.DisposeAsync();
            return Task.CompletedTask;
        }
    }
}