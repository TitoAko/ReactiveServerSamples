using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Tests.Communication
{
    internal class TcpIntegrationTests
    {
        public class TcpIntegrationTests : IAsyncLifetime
        {
            private readonly int _port = PortFinder.GetFreePort();
            private readonly Configuration _cfg;
            private readonly TcpCommunicator _server;
            private readonly TcpCommunicator _client;

            private readonly List<Message> _received = new();

            public TcpIntegrationTests()
            {
                _cfg = new Configuration { BindAddress = "0.0.0.0", TargetAddress = "127.0.0.1", Port = _port, Transport = TransportKind.Tcp };
                _server = new TcpCommunicator(_cfg);
                _client = new TcpCommunicator(_cfg);
                _server.MessageReceived += (_, m) => _received.Add(m);
            }

            public async Task InitializeAsync()
            {
                await _server.StartAsync();
                await _client.StartAsync();
            }
            public Task DisposeAsync()
            {
                _server.Dispose();
                _client.Dispose();
                return Task.CompletedTask;
            }

            [Fact(Timeout = 2000)]
            public async Task SendAndReceive_SingleChatMessage()
            {
                var msg = new Message("cli", "hello", new TextMessage());
                await _client.SendMessageAsync(msg);
                await TaskTimeoutExtensions.WaitForMessageAsync(_received, 1, 1000);
                _received.Single().Content.Should().Be("hello");
            }
        }

    }
}
