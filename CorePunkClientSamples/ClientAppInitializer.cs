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
        private readonly IClient _chatClient;
        private readonly ICommunicator _communicator;

        public ClientAppInitializer()
        {
            _config = new Configuration("launchSettings.json");  // Use AppConfiguration to hold all the parameters
            _loggingService = new LoggingService();
            _appLock = new AppLock();
            _outputHandler = new OutputHandler();
            _clientHandler = new ClientHandler();

            _communicator = CreateCommunicatorFromConfig(_config.Communicator);

            _chatClient = new ChatClient(_clientHandler, _outputHandler, _config, _communicator);

            InitializeClient();
        }

        private ICommunicator CreateCommunicatorFromConfig(string communicatorType)
        {
            Type? communicatorClass = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.IsClass
                && typeof(ICommunicator).IsAssignableFrom(type)
                && type.Name.Equals(communicatorType, StringComparison.OrdinalIgnoreCase)
                && type.Namespace == "CoreLibrary.Communication");
            if (communicatorClass == null)
            {
                throw new ArgumentException($"Invalid communicator type: {communicatorType}");
            }
            ICommunicator? communicator;
            try
            {
                communicator = (ICommunicator?)Activator.CreateInstance(communicatorClass, _config.IpAddress, _config.Port);

                // Create an instance of the communicator class
                if (communicator == null)
                {
                    throw new InvalidOperationException($"Failed to create instance of communicator type: {communicatorType}");
                }
                return communicator;
            }
            catch
            {
                throw new InvalidOperationException($"Failed to create instance of communicator type: {communicatorType}");
            }
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
            _clientHandler?.Disconnect();
            _outputHandler?.DisposeResources();  // Dispose of the OutputHandler if it implements IDisposable
            _outputHandler?.Dispose();
        }
    }
}