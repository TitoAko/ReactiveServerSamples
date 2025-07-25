using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Messaging;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;
using Xunit;

namespace CoreLibrary.Tests.EdgeCases;

public class TcpZeroLengthPayloadTests : IAsyncLifetime
{
    private TcpCommunicator? _server;
    private TcpCommunicator? _client;

    public async Task InitializeAsync()
    {
        var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());
        _server = new TcpCommunicator(cfg);
        await _server.StartAsync();
        _client = new TcpCommunicator(cfg);
        await _client.StartAsync();
    }

    [Fact]
    public async Task EmptyMessage_RoundTrips()
    {
        var list = new List<Message>();
        _server!.MessageReceived += (_, m) => list.Add(m);

        await _client!.SendMessageAsync(new Message("cli", ""));

        await TaskTimeoutExtensions.WaitForMessageAsync(list, 1, 1000);
        Assert.Equal(string.Empty, list.Single().Content);
    }

    public Task DisposeAsync()
    {
        _server?.Dispose();
        _client?.Dispose();
        return Task.CompletedTask;
    }
}
