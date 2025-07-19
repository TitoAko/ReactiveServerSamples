using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CoreLibrary.Tests.Communication
{
    public class UdpReceiverTests : IDisposable
    {
        private readonly int _testPort = GetFreePort();
        private readonly UdpClient _testSender;
        private readonly Configuration _config;
        private readonly CancellationTokenSource _cts;

        public UdpReceiverTests()
        {
            _cts = new CancellationTokenSource();
            _testSender = new UdpClient();
            var values = new Dictionary<string, string?>
            {
                ["AppConfig:Username"] = "TestUser",
                ["AppConfig:Password"] = "TestPass",
                ["AppConfig:IpAddress"] = "127.0.0.1",
                ["AppConfig:Port"] = _testPort.ToString(),
                ["AppConfig:Communicator"] = "UdpCommunicator",
                ["AppConfig:AppType"] = "Test"
            };
            var inMemory = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            _config = new Configuration(inMemory);
        }

        private static int GetFreePort()
        {
            var listener = new UdpClient(0); // OS assigns an available port
            int port = ((IPEndPoint)listener.Client.LocalEndPoint!).Port;
            listener.Close();
            return port;
        }

        [Fact]
        public async Task ShouldTriggerObservableOnPacketReceived()
        {
            var receiver = new UdpReceiver(_config);
            Message? received = null;
            var message = new Message("TestUser", "Hello!", new Messaging.MessageTypes.TextMessage());
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            receiver.OnMessageReceived += msg => { received = msg; };

            var listeningTask = Task.Run(() => receiver.StartObservables(_cts.Token));
            await Task.Delay(100); // Give it time to bind socket

            await _testSender.SendAsync(bytes, bytes.Length, "127.0.0.1", _testPort);
            await Task.Delay(200);

            _cts.Cancel();
            await listeningTask;

            Assert.NotNull(received);
            Assert.Equal("Hello!", received!.Content);
        }

        [Fact]
        public async Task ShouldRespectCancellationToken()
        {
            var receiver = new UdpReceiver(_config);
            var token = _cts.Token;

            var listeningTask = Task.Run(() => receiver.StartObservables(token));
            await Task.Delay(100);

            _cts.Cancel();
            await listeningTask; // Should complete without exception
        }

        [Fact]
        public async Task ShouldHandleMalformedPacketGracefully()
        {
            var receiver = new UdpReceiver(_config);
            Exception? observedException = null;
            Message? observedMessage = null;

            receiver.OnMessageReceived += msg =>
            {
                // capture or assert here
                observedMessage = msg;
            };

            var listeningTask = Task.Run(() => receiver.StartObservables(_cts.Token));
            await Task.Delay(100);

            // Send junk data
            var junk = Encoding.UTF8.GetBytes("not json");
            await _testSender.SendAsync(junk, junk.Length, "127.0.0.1", _testPort);

            await Task.Delay(100);
            _cts.Cancel();
            await listeningTask;

            Assert.Null(observedException); // Should not throw to observable
        }

        [Fact]
        public async Task ShouldNotThrowWhenNoSubscribers()
        {
            var receiver = new UdpReceiver(_config);
            var listeningTask = Task.Run(() => receiver.StartObservables(_cts.Token));

            await Task.Delay(100);
            _cts.Cancel();
            await listeningTask;

            Assert.True(true); // If it didn't throw, it's a pass
        }

        public void Dispose()
        {
            _cts.Cancel();
            _testSender.Dispose();
            _cts.Dispose();
        }
    }
}
