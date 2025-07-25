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
    public async Task EmptyMessage_IsRejected()
    {
        var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());
        using var s = new TcpSender(cfg);

        var empty = new Message("cli", "");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.SendAsync(empty).TimeoutAfter(TimeSpan.FromSeconds(1)));
    }

    public Task DisposeAsync()
    {
        _server?.Dispose();
        _client?.Dispose();
        return Task.CompletedTask;
    }
}
