using ClientApp;
using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;
using ServerApp;

namespace CorePunkClientSamples
{
    public class ClientAppInitializer
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Configuration _config;
        private readonly LoggingService _loggingService; // using the centralized LoggingService
        private readonly IMessageProcessor _messageProcessor;
        private readonly OutputHandler _outputHandler;
        private readonly InputHandler _inputHandler;
        private readonly ClientHandler _clientHandler;
        private readonly ICommunicator _communicator; //communicator to allow flexibility for Udp/Tcp or any other communication
        private readonly AppLock _appLock; // Inject ClientLock to manage client instances

        public ClientAppInitializer(string username, string password, LoggingService loggingService, ICommunicator communicator, ClientHandler clientHandler, AppLock appLock)
        {
            _username = username;
            _password = password;
            _config = new Configuration("launchSettings.json");
            _loggingService = loggingService;
            _clientHandler = clientHandler;
            _appLock = appLock;
            _outputHandler = new OutputHandler();
            _inputHandler = new InputHandler(new ChatClient(_clientHandler, _outputHandler, _config));
            _communicator = communicator; // store the communicator (UDP/TCP) for sending/receiving messages
        }

        // Authenticate the user and initialize the client
        public bool InitializeClient()
        {
            // Authenticate the user
            if (!AuthenticateUser())
            {
                _loggingService.Log("Authentication failed!");
                return false;
            }

            // Check if client is already running for the given user/IP/port
            if (_appLock.IsInstanceRunning(_username, _config.ServerIpAddress, _config.ServerPort))
            {
                _loggingService.Log("Another instance of the client is already running for this user on the given IP/Port.");
                return false;
            }

            // initialize the client
            var chatClient = new ChatClient(_clientHandler, _outputHandler, _config);
            chatClient.Connect();
            chatClient.StartListening();

            // Client initialized successfully
            return true;
        }

        // Authenticate the user using username and password
        private bool AuthenticateUser()
        {
            return UserAuthenticator.Authenticate(_username, _password);
        }

        // Release the lock after the client finishes
        public void ReleaseLock()
        {
            ClientLock.ReleaseLock();
        }
    }
}