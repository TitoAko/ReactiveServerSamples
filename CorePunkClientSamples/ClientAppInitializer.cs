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
            var commandLineParser = new Cli(args);           // use any simple CLI parser you like
            var configuration = new Configuration
            {
                BindAddress = commandLineParser.Get("--bind", "0.0.0.0"),
                TargetAddress = commandLineParser.Get("--target", "server"),
                Port = commandLineParser.Get("--port", 9000),
                Role = NodeRole.Server
            };

            ICommunicator comm = new UdpCommunicator(configuration);

            using var chatClient = new ChatClient(comm, configuration.ClientId);
            await chatClient.RunAsync();
        }
    }
}