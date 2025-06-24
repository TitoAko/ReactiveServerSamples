using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;
using CorePunkClientSamples;
using ServerApp;

public class Program
{
    public static void Main(string[] args)
    {
        var credentials = Login.GetUserCredentials();
        if (credentials == null) return;

        string username = credentials.Value.username;
        string password = credentials.Value.password;

        // Initialize LoggingService
        LoggingService loggingService = new LoggingService();

        // Choose communicator (Udp or Tcp)
        ICommunicator communicator = new UdpCommunicator("127.0.0.1", 8080, new MessageProcessor(loggingService, new ChatServer(new UserManager())));

        // Initialize AppLock
        AppLock appLock = new AppLock();

        // Initialize ClientAppInitializer
        ClientAppInitializer initializer = new ClientAppInitializer(username, password, loggingService, communicator, appLock);

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
