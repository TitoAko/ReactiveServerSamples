using CoreLibrary.Factories;
using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ClientApp
{
    /// <summary>
    /// Responsible for setting up the client environment, including configuration, logging, authentication, and lock enforcement.
    /// </summary>
    public class ClientAppInitializer : IDisposable
    {
        private readonly Configuration _config;
        private readonly LoggingService _loggingService;
        private readonly OutputHandler _outputHandler;
        private readonly ClientHandler _clientHandler;
        private readonly AppLock _appLock;
        private readonly ICommunicator _communicator;
        private readonly IClient _chatClient;

        /// <summary>
        /// Initializes the application and sets up all core components.
        /// </summary>
        public ClientAppInitializer()
        {
            _config = new Configuration("launchSettings.json");  // Use AppConfiguration to hold all the parameters
            _loggingService = new LoggingService();
            _appLock = new AppLock();
            _outputHandler = new OutputHandler();
            _clientHandler = new ClientHandler();

            _communicator = CommunicatorFactory.Create(_config);

            _chatClient = new ChatClient(_clientHandler, _outputHandler, _communicator, _config.Username);

            InitializeClient();
            Console.WriteLine("ClientAppInitializer initialized successfully. ");
        }

        /// <summary>
        /// Performs authentication, checks lock, and connects the client.
        /// </summary>
        public bool InitializeClient()
        {
            if (!AuthenticateUser())
            {
                _loggingService.Log("Authentication failed!");
                return false;
            }

            // Use AppLock to check if the client is already running
            if (_appLock.IsInstanceRunning(_config))
            {
                _loggingService.Log("Another instance of the client is already running for this user on the given IP/Port.");
                return false;
            }

            _loggingService.Log("Client initialized successfully.");


            _chatClient.Connect(); // Connect the client to the server and start listening for messages

            return true;
        }

        /// <summary>
        /// Authenticates the user based on configuration.
        /// </summary>
        private bool AuthenticateUser()
        {
            return UserAuthenticator.Authenticate(_config.Username, _config.Password);
        }

        /// <summary>
        /// Releases the application-level lock.
        /// </summary>
        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the client finishes
        }

        /// <summary>
        /// Releases the application lock, disposes internal resources, and performs cleanup operations.
        /// </summary>
        public void Dispose()

        {
            ReleaseLock();
            _communicator?.Dispose();
            _outputHandler?.Dispose();
            _clientHandler?.Disconnect();
        }
    }
}