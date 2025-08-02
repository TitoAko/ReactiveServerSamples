using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ServerApp
{
    internal static class ServerAppInitializer
    {
        /// <summary>Loads config, creates communicator + ChatServer, blocks until Ctrl-C.</summary>
        public static void Run(string[] args)
        {
            var cfg = ConfigurationLoader.Load(args);

            // enforce correct role
            if (cfg.Role != NodeRole.Server)
            {
                cfg = cfg with { Role = NodeRole.Server };
            }

            ICommunicator comm = new UdpCommunicator(cfg);

            // ChatServer ctor **starts** listening immediately :contentReference[oaicite:2]{index=2}
            new ChatServer().AddClient(cfg);

            Console.WriteLine($"UDP server listening on {cfg.BindAddress}:{cfg.Port}. Press Ctrl-C to exit.");

            // Keep process alive
            ManualResetEventSlim blocker = new();
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