using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;

namespace CoreLibrary.Tests.Integration
{

    public sealed class UdpIntegrationTests : IAsyncLifetime
    {
        private UdpCommunicator? _a, _b;
        private readonly List<Message> _aReceived = new();
        private readonly List<Message> _bReceived = new();
        private int _port;

        public async Task InitializeAsync()
        {
            int portA = PortFinder.FreePort();
            int portB = PortFinder.FreePort();

            var cfgA = new Configuration { IpAddress = "127.0.0.1", Port = portA };
            var cfgB = new Configuration { IpAddress = "127.0.0.1", Port = portB };

            _a = new UdpCommunicator(cfgA, remotePort: portB); // A sends to B
            _b = new UdpCommunicator(cfgB, remotePort: portA); // B sends to A

            _a.MessageReceived += (_, m) => _aReceived.Add(m);
            _b.MessageReceived += (_, m) => _bReceived.Add(m);

            _ = _a.StartAsync();
            _ = _b.StartAsync();

            await Task.Delay(50); // allow sockets to bind
        }

        public Task DisposeAsync()
        {
            _a?.Dispose();
            _b?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task SendAndReceive_SingleChatMessage()
        {
            var msg = new Message("alice", "ping", MessageType.Chat);
            await _a!.SendMessageAsync(msg);

            await WaitUntil(() => _bReceived.Count == 1);
            Assert.Equal("ping", _bReceived[0].Content);
        }

        [Fact]
        public async Task SendAndReceive_ExitMessage()
        {
            var msg = new Message("bob", "<bye>", MessageType.Exit);
            await _b!.SendMessageAsync(msg);

            await WaitUntil(() => _aReceived.Any(m => m.Type == MessageType.Exit));
            Assert.Equal(MessageType.Exit, _aReceived.Last().Type);
        }

        [Fact]
        public async Task Messages_Preserve_Order()
        {
            for (int i = 0; i < 5; i++)
                await _a!.SendMessageAsync(new Message("seq", $"#{i}", MessageType.Chat));

            await WaitUntil(() => _bReceived.Count >= 5);
            var contents = _bReceived.Take(5).Select(m => m.Content).ToArray();
            Assert.Equal(new[] { "#0", "#1", "#2", "#3", "#4" }, contents);
        }

        // ------------------------------------------------------------
        private static async Task WaitUntil(Func<bool> condition, int ms = 1000)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > ms)
                    throw new TimeoutException("Condition not met in time.");
                await Task.Delay(10);
            }
        }
    }
}
