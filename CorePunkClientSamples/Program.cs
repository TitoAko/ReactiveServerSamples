using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;
//using ServerApp;

namespace ClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Configuration configuration = new Configuration("config.json");

            // Initialize LoggingService
            LoggingService loggingService = new LoggingService();

            // Initialize AppLock
            AppLock appLock = new AppLock();

            ClientHandler clientHandler = new ClientHandler();

            OutputHandler outputHandler = new OutputHandler();

            // Choose communicator (Udp or Tcp)
            ICommunicator communicator = new UdpCommunicator(configuration.IpAddress, configuration.Port, MessageProcessorFactory.CreateProcessor(loggingService, new ChatClient(clientHandler, outputHandler, configuration)));

            // Initialize ClientAppInitializer
            ClientAppInitializer initializer = new ClientAppInitializer(configuration, loggingService, communicator, appLock, clientHandler);

            // Initialize the client
            if (initializer.InitializeClient())
            {
                loggingService.Log("Client initialized successfully.");
            }
            else
            {
                loggingService.Log("Client initialization failed.");
                return;
            }

            // Release lock after finishing
            initializer.ReleaseLock();
        }
    }
}