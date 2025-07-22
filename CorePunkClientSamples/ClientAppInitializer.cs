using CoreLibrary.Communication.UdpCommunication;   // Communicator
using CoreLibrary.Utilities;                        // Configuration

namespace ClientApp
{
    internal static class ClientAppInitializer
    {
        /// <summary>
        /// Creates a communicator, spins up the CLI, blocks until user types "exit".
        /// </summary>
        public static async Task RunAsync(string[] args)
        {
            // Allow "chat.exe 10.0.0.2 5555"
            var cfg = new Configuration
            {
                IpAddress = args.Length > 0 ? args[0] : "127.0.0.1",
                Port = args.Length > 1 && int.TryParse(args[1], out var p) ? p : 9000,
                ClientId = $"cli-{Guid.NewGuid():N}",          // unique per console
                Transport = TransportKind.Udp
            };

            ICommunicator comm = new UdpCommunicator(cfg);

            using var chat = new ChatClient(comm, cfg.ClientId);
            await chat.RunAsync();
        }
    }
}
