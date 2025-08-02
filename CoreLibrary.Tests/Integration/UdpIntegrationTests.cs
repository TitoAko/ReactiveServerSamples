using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Integration
{

    public sealed class UdpIntegrationTests : IAsyncLifetime
    {
        private readonly List<Message> _firstMessageReceived = new();
        private readonly List<Message> _secondMessageReceived = new();
        private readonly CancellationTokenSource _cts = new();
        private UdpCommunicator? _server;
        private UdpCommunicator? _client;

        public async Task InitializeAsync()
        {
            var serverPort = PortFinder.FreePort();
            var clientPort = PortFinder.FreePort();

            // Server config (listens on serverPort)
            var serverCfg = new Configuration
            {
                Role = NodeRole.Server,
                Communicator = "UdpCommunicator",
                BindAddress = "127.0.0.1",
                TargetAddress = "127.0.0.1",
                Port = serverPort,
                Username = "server",
                Password = "server"
            };

            // Client config (binds clientPort, targets serverPort)
            var clientCfg = serverCfg with
            {
                Role = NodeRole.Client,
                Port = clientPort
            };

            _server = new UdpCommunicator(serverCfg, clientPort);               // local bind clientPort, sends to serverPort
            _client = new UdpCommunicator(clientCfg, serverPort);               // local bind serverPort, sends to clientPort

            await Task.Delay(100); // Give time for the server to start, add await to remove warning cs1998
            _ = _server.StartAsync();
            _ = _client.StartAsync();

            _server!.MessageReceived += (_, message) => _firstMessageReceived.Add(message);
            _client!.MessageReceived += (_, message) => _secondMessageReceived.Add(message);
        }

        public async Task DisposeAsync()
        {
            _cts.Cancel(); // Cancel any ongoing operations
            await _server!.DisposeAsync()!;
            await _client!.DisposeAsync()!;
        }

        [Fact]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var msg = new Message("alice", "ping", MessageType.Chat);
            await _client!.SendMessageAsync(msg);

            await WaitUntil(() => _firstMessageReceived.Count == 1, 2000);
            Assert.Equal("ping", _firstMessageReceived[0].Content);
        }

        [Fact]
        public async Task SendAndReceive_ExitMessage()
        {
            var message = new Message("bob", "<bye>", MessageType.Exit);
            await _client!.SendMessageAsync(message);

            await WaitUntil(() => _firstMessageReceived.Any(messageReceived => messageReceived.Type == MessageType.Exit));
            Assert.Equal(MessageType.Exit, _firstMessageReceived.Last().Type);
        }

        [Fact]
        public async Task Messages_Preserve_Order()
        {
            for (int i = 0; i < 5; i++)
            {
                await _server!.SendMessageAsync(new Message("seq", $"#{i}", MessageType.Chat));
            }

            await WaitUntil(() => _secondMessageReceived.Count >= 5);
            var contents = _secondMessageReceived.Take(5).Select(m => m.Content).ToArray();
            Assert.Equal(new[] { "#0", "#1", "#2", "#3", "#4" }, contents);
        }

        // ------------------------------------------------------------
        private static async Task WaitUntil(Func<bool> condition, int ms = 1000)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > ms)
                {
                    throw new TimeoutException("Condition not met in time.");
                }

                await Task.Delay(10);
            }
        }
    }
}
