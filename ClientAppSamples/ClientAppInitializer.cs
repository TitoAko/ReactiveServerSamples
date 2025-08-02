using CoreLibrary.Communication.UdpCommunication;   // Communicator
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;                        // Configuration

namespace ClientApp
{
    internal static class ClientAppInitializer
    {
        /// <summary>
        /// Loads configuration, creates a communicator, spins up the CLI, and blocks until user exits.
        /// </summary>
        public static async Task RunAsync(string[] args)
        {
            // 1️⃣  Load layered configuration (JSON → ENV → CLI)
            var cfg = ConfigurationLoader.Load(args);

            // 2️⃣  Guard-rail: force correct role
            if (cfg.Role != NodeRole.Client)
            {
                cfg = cfg with { Role = NodeRole.Client };
            }

            // 3️⃣  Create communicator
            ICommunicator comm = new UdpCommunicator(cfg);

            // 4️⃣  Spin up client
            using var chatClient = new ChatClient(comm, cfg.Username);
            await chatClient.RunAsync();
        }
    }
}