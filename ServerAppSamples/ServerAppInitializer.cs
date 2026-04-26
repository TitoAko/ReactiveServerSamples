using CoreLibrary.Utilities;

namespace ServerApp
{
    internal static class ServerAppInitializer
    {
        /// <summary>Loads config, starts the UDP server, blocks until Ctrl-C.</summary>
        public static void Run(string[] args)
        {
            var cfg = ConfigurationLoader.Load(args);

            if (cfg.Role != NodeRole.Server)
            {
                cfg = cfg with { Role = NodeRole.Server };
            }

            var server = new ChatServer();

            // In a real app, you'd likely have more complex logic to choose the communicator type (UDP/TCP) and other settings.
            server.AddClient(cfg);

            Console.WriteLine($"UDP server listening on {cfg.BindAddress}:{cfg.ListenPort}. Press Ctrl-C to exit.");

            using ManualResetEventSlim blocker = new();

            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("Shutdown requested…");
                e.Cancel = true;
                blocker.Set();
            };

            blocker.Wait();
        }
    }
}