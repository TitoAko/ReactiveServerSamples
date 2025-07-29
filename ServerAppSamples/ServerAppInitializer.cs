using CoreLibrary.Utilities;

namespace ServerApp
{
    /// <summary>
    /// Bootstraps the chat server: loads config, spins up a communicator via
    /// <see cref="ChatServer"/>, and handles Ctrl-C shutdown.
    /// </summary>
    public sealed class ServerAppInitializer
    {
        private readonly Configuration _configurationg;
        private readonly ChatServer _server = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public ServerAppInitializer(string[] args)
        {
            var commandLineParser = new Cli(args);           // use any simple CLI parser you like
            _configurationg = new Configuration
            {
                BindAddress = commandLineParser.Get("--bind", "0.0.0.0"),
                TargetAddress = commandLineParser.Get("--target", "server"),
                Port = commandLineParser.Get("--port", 9000),
                Role = NodeRole.Server
            };

        }

        public async Task RunAsync()
        {
            Console.WriteLine($"[Server] Starting on UDP {_configurationg.IpAddress}:{_configurationg.Port}");

            await _server.AddClientAsync(_configurationg); // creates UDP listener & waits

            Console.WriteLine("[Server] Press Ctrl-C to exit.");
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                _cancellationTokenSource.Cancel();
            };

            try { await Task.Delay(-1, _cancellationTokenSource.Token); }
            catch (TaskCanceledException) { /* graceful */ }

            Console.WriteLine("[Server] Shutting down.");
        }
    }
}