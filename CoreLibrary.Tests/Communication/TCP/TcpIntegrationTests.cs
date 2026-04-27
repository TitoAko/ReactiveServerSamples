using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;

using FluentAssertions;

namespace CoreLibrary.Tests.Communication.TCP
{
    public class TcpIntegrationTests
    {
        [Fact(Timeout = 3000)]
        public async Task ClientMessage_ReachesServer()
        {
            int port = PortFinder.FreePort();

            var serverCfg = TestConfig.TcpLoopback(port);
            var clientCfg = TestConfig.TcpLoopback(port);

            await using var server = new TcpCommunicator(serverCfg);
            await using var client = new TcpCommunicator(clientCfg);

            var received = new List<Message>();
            server.MessageReceived += (_, m) => received.Add(m);

            await server.StartAsync();
            await server.Started;

            await client.SendMessageAsync(new Message("cli", "hello"));

            await TaskTimeoutExtensions.WaitForMessageAsync(received, 1, 1000);

            received.Single().Content.Should().Be("hello");
        }
    }
}
