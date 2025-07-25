using CoreLibrary.Communication.TcpCommunication;
using CoreLibrary.Tests.TestInfrastructure;
using CoreLibrary.Utilities;
using System.Net.Sockets;
using Xunit;

namespace CoreLibrary.Tests.EdgeCases;

public class TcpHalfCloseTests : IAsyncLifetime
{
    private TcpCommunicator? _server;
    private TcpClient? _raw;

    public async Task InitializeAsync()
    {
        var cfg = TestConfig.TcpLoopback(PortFinder.FreePort());
        _server = new TcpCommunicator(cfg);
        await _server.StartAsync();

        // raw client socket to simulate half-close
        _raw = new TcpClient();
        _raw.Connect("127.0.0.1", cfg.Port);
    }

    [Fact(Timeout = 2000)]
    public async Task ReadLoop_Exits_On_RemoteClose()
    {
        _raw?.Client.Shutdown(SocketShutdown.Send);   // half-close
        await Task.Delay(100);                       // give server time to read 0 bytes

        // dispose should not hang (listener loop must have exited)
        var cts = new CancellationTokenSource(500);
        await Task.Run(() => _server!.Dispose(), cts.Token);
        Assert.False(cts.IsCancellationRequested);
    }

    public Task DisposeAsync()
    {
        _raw?.Dispose();
        _server?.Dispose();
        return Task.CompletedTask;
    }
}
