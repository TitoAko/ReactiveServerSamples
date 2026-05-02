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

            var serverCfg = new Configuration
            {
                Role = NodeRole.Server,
                Communicator = "UdpCommunicator",
                BindAddress = "127.0.0.1",
                ListenPort = serverPort,
                TargetAddress = "127.0.0.1",
                TargetPort = clientPort,
                Username = "server",
                Password = "server"
            };

            var clientCfg = new Configuration
            {
                Role = NodeRole.Client,
                Communicator = "UdpCommunicator",
                BindAddress = "127.0.0.1",
                ListenPort = clientPort,
                TargetAddress = "127.0.0.1",
                TargetPort = serverPort,
                Username = "client",
                Password = "client"
            };

            _server = new UdpCommunicator(serverCfg);
            _client = new UdpCommunicator(clientCfg);

            _server.MessageReceived += (_, message) => _firstMessageReceived.Add(message);
            _client.MessageReceived += (_, message) => _secondMessageReceived.Add(message);

            await _server.StartAsync(_cts.Token);
            await _client.StartAsync(_cts.Token);
        }

        public async Task DisposeAsync()
        {
            _cts.Cancel();

            if (_server is not null)
            {
                await _server.DisposeAsync();
            }

            if (_client is not null)
            {
                await _client.DisposeAsync();
            }

            _cts.Dispose();
        }

        [Fact(Timeout = 3000)]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var msg = new Message("alice", "ping", MessageType.Chat);
            await _client!.SendMessageAsync(msg);

            await WaitUntil(() => _firstMessageReceived.Count == 1);
            Assert.Equal("ping", _firstMessageReceived[0].Content);
        }

        [Fact(Timeout = 3000)]
        public async Task SendAndReceive_ExitMessage()
        {
            var message = new Message("bob", "<bye>", MessageType.Exit);
            await _client!.SendMessageAsync(message);

            await WaitUntil(() => _firstMessageReceived.Any(messageReceived => messageReceived.Type == MessageType.Exit));
            Assert.Equal(MessageType.Exit, _firstMessageReceived.Last().Type);
        }

        [Fact(Timeout = 3000)]
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
        private static async Task WaitUntil(Func<bool> condition, int ms = 2000)
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
