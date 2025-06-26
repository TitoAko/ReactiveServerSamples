using CoreLibrary.Handlers;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;
using ServerApp;

namespace ClientApp
{
    public class ClientAppInitializer
    {
        private readonly Configuration _config;
        private readonly LoggingService _loggingService;
        private readonly OutputHandler _outputHandler;
        private readonly InputHandler _inputHandler;
        private readonly ICommunicator _communicator;
        private readonly ClientHandler _clientHandler;
        private readonly AppLock _appLock;

        public ClientAppInitializer(Configuration config, LoggingService loggingService, ICommunicator communicator, AppLock appLock, ClientHandler clientHandler)
        {
            _config = config;  // Use AppConfiguration to hold all the parameters
            _loggingService = loggingService;
            _appLock = appLock;
            _outputHandler = new OutputHandler();
            _communicator = communicator;
            _clientHandler = clientHandler;
            _inputHandler = new InputHandler(new ChatClient(_clientHandler, _outputHandler, new Configuration("launchSettings.json")));
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

            // Initialize the MessageProcessor with LoggingService and IBroadcastMessage (ChatServer)
            var messageProcessor = MessageProcessorFactory.CreateProcessor(_loggingService, new ChatClient(_clientHandler, _outputHandler, _config));

            // Create and start the client
            var chatClient = new ChatClient(_clientHandler, _outputHandler, new Configuration("launchSettings.json"));
            chatClient.Connect();
            chatClient.StartListening();  // Start listening for messages

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
    }
}