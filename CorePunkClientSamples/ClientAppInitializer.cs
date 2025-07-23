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
            var cli = new Cli(args);           // use any simple CLI parser you like
            var cfg = new Configuration
            {
                BindAddress = cli.Get("--bind", "0.0.0.0"),
                TargetAddress = cli.Get("--target", "server"),
                Port = cli.Get("--port", 9000),
                Role = NodeRole.Server
            };


            ICommunicator comm = new UdpCommunicator(cfg);

            using var chat = new ChatClient(comm, cfg.ClientId);
            await chat.RunAsync();
        }
    }
}