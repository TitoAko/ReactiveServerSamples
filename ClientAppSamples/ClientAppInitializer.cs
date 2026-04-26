using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ClientApp
{
    internal static class ClientAppInitializer
    {
        /// <summary>
        /// Loads configuration, creates a communicator, starts the client, and blocks until Ctrl-C.
        /// </summary>
        public static async Task RunAsync(string[] args)
        {
            var cfg = ConfigurationLoader.Load(args); // In a real app, we'd likely have more complex logic to choose the communicator type (UDP/TCP) and other settings.

            var nonInteractive = Environment.GetEnvironmentVariable("CHAT_NONINTERACTIVE") == "true"; // For testing: skip the interactive client if this env var is set, to avoid blocking test runs.

            if (cfg.Role != NodeRole.Client)
            {
                cfg = cfg with { Role = NodeRole.Client };
            }

            ICommunicator comm = new UdpCommunicator(cfg);

            using var chatClient = new ChatClient(comm, cfg.Username);

            var runTask = chatClient.RunAsync(nonInteractive);

            Console.WriteLine($"UDP client listening on {cfg.BindAddress}:{cfg.ListenPort}");
            Console.WriteLine($"UDP client sending to {cfg.TargetAddress}:{cfg.TargetPort}");
            Console.WriteLine("Client started. Press Ctrl-C to exit.");

            using ManualResetEventSlim blocker = new();

            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("Shutdown requested…");
                e.Cancel = true;
                blocker.Set();
            };

            blocker.Wait();

            await comm.DisposeAsync();
        }
    }
}