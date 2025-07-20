using CoreLibrary.Factories;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ServerApp
{
    /// <summary>
    /// Responsible for initializing the server, including config loading, locking, and starting communication observables.
    /// </summary>
    public class ServerAppInitializer
    {
        private readonly Configuration _config;
        private readonly AppLock _appLock;
        private readonly ICommunicator _communicator;
        private CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// Initializes configuration, communication system, and app locking.
        /// </summary>
        public ServerAppInitializer()
        {
            _config = new Configuration("launchSettings.json");  // Use Configuration to hold all the parameters
            _appLock = new AppLock();
            _communicator = CommunicatorFactory.Create(_config);
        }

        /// <summary>
        /// Performs the main server setup logic and starts message listening.
        /// </summary>
        public void InitializeServer()
        {
            // Using ClientLock to check if the server is already running
            if (_appLock.IsInstanceRunning(_config))
            {
                Console.WriteLine("The server is already running on this IP/Port.");
            }

            // Initialize the server logic here...
            // Start the server
            Console.WriteLine("Server is starting...");

            StartObservables();
        }

        /// <summary>
        /// Releases the instance lock to allow another server instance to start.
        /// </summary>
        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the server finishes
        }

        /// <summary>
        /// Starts the reactive observables and message handling logic.
        /// </summary>
        public void StartObservables()
        {
            Console.WriteLine($"UDP server is listening on port {_config.Port}...");

            // initialize ChatServer
            var chatServer = new ChatServer(new UserManager());

            _communicator.StartListening(_cancellationTokenSource.Token); // TODO: add real cancellation

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Shutting down.");

                _cancellationTokenSource.Cancel();  // Signal cancellation to observables
                _communicator.Stop();                // Clean up Rx
                _communicator.Dispose();             // Final cleanup
                ReleaseLock();
            };
        }
    }
}
