using CoreLibrary.Factories;
using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ClientApp
{
    public class ClientAppInitializer : IDisposable
    {
        private readonly Configuration _config;
        private readonly LoggingService _loggingService;
        private readonly OutputHandler _outputHandler;
        private readonly ClientHandler _clientHandler;
        private readonly AppLock _appLock;
        private readonly ICommunicator _communicator;
        private readonly IClient _chatClient;

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

        private bool AuthenticateUser()
        {
            return UserAuthenticator.Authenticate(_config.Username, _config.Password);
        }

        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the client finishes
        }

        public void Dispose()
        {
            ReleaseLock();
            _communicator?.Dispose();
            _outputHandler?.Dispose();
            _clientHandler?.Disconnect();
        }
    }
}