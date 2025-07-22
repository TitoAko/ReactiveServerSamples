using CoreLibrary.Utilities;
using ServerApp;

namespace ServerApp;

/// <summary>
/// Bootstraps the chat server: loads config, spins up a communicator via
/// <see cref="ChatServer"/>, and handles Ctrl-C shutdown.
/// </summary>
public sealed class ServerAppInitializer
{
    private readonly Configuration _cfg;
    private readonly ChatServer _server = new();
    private readonly CancellationTokenSource _cts = new();

    public ServerAppInitializer(string? ip = null, int? port = null)
    {
        _cfg = new Configuration
        {
            IpAddress = ip ?? "127.0.0.1",
            Port = port ?? 9000,
            Role = NodeRole.Server
        };
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"[Server] Starting on UDP {_cfg.IpAddress}:{_cfg.Port}");

        await _server.AddClientAsync(_cfg); // creates UDP listener & waits

        Console.WriteLine("[Server] Press Ctrl-C to exit.");
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _cts.Cancel();
        };

        try { await Task.Delay(-1, _cts.Token); }
        catch (TaskCanceledException) { /* graceful */ }

        Console.WriteLine("[Server] Shutting down.");
    }
}
