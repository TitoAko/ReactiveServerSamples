using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Integration
{

    public sealed class UdpIntegrationTests : IAsyncLifetime
    {
        private UdpCommunicator? _firstCommunicator, _secondCommunicator;
        private readonly List<Message> _firstMessageReceived = new();
        private readonly List<Message> _secondMessageReceived = new();

        public async Task InitializeAsync()
        {
            int senderPort = PortFinder.FreePort();
            int communicatorPort = PortFinder.FreePort();

            var senderConfiguration = new Configuration { IpAddress = "127.0.0.1", Port = senderPort };
            var communicatorConfiguration = new Configuration { IpAddress = "127.0.0.1", Port = communicatorPort };

            _firstCommunicator = new UdpCommunicator(senderConfiguration, remotePort: communicatorPort); // A sends to B
            _secondCommunicator = new UdpCommunicator(communicatorConfiguration, remotePort: senderPort); // B sends to A

            _firstCommunicator.MessageReceived += (_, message) => _firstMessageReceived.Add(message);
            _secondCommunicator.MessageReceived += (_, message) => _secondMessageReceived.Add(message);

            _ = _firstCommunicator.StartAsync();
            _ = _secondCommunicator.StartAsync();

            await Task.Delay(50); // allow sockets to bind
        }

        public Task DisposeAsync()
        {
            _firstCommunicator?.Dispose();
            _secondCommunicator?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var msg = new Message("alice", "ping", MessageType.Chat);
            await _firstCommunicator!.SendMessageAsync(msg);

            await WaitUntil(() => _secondMessageReceived.Count == 1);
            Assert.Equal("ping", _secondMessageReceived[0].Content);
        }

        [Fact]
        public async Task SendAndReceive_ExitMessage()
        {
            var message = new Message("bob", "<bye>", MessageType.Exit);
            await _secondCommunicator!.SendMessageAsync(message);

            await WaitUntil(() => _firstMessageReceived.Any(messageReceived => messageReceived.Type == MessageType.Exit));
            Assert.Equal(MessageType.Exit, _firstMessageReceived.Last().Type);
        }

        [Fact]
        public async Task Messages_Preserve_Order()
        {
            for (int i = 0; i < 5; i++)
            {
                await _firstCommunicator!.SendMessageAsync(new Message("seq", $"#{i}", MessageType.Chat));
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
