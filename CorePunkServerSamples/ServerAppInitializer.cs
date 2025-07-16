using CoreLibrary.Communication.UdpCommunication;
using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace ServerApp
{
    public class ServerAppInitializer
    {
        private readonly Configuration _config;
        private readonly AppLock _appLock;
        private readonly ICommunicator _communicator;
        private CancellationTokenSource _cancellationTokenSource = new();

        public ServerAppInitializer()
        {
            _config = new Configuration("launchSettings.json");  // Use Configuration to hold all the parameters
            _appLock = new AppLock();
            _communicator = new UdpCommunicator(_config);
        }

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

        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the server finishes
        }

        public void StartObservables()
        {
            Console.WriteLine($"UDP server is listening on port {_config.Port}...");

            // initialize ChatServer
            var chatServer = new ChatServer(new UserManager());
            var udpReceiver = _communicator as UdpCommunicator;

            udpReceiver?.StartListening(_cancellationTokenSource.Token); // TODO: add real cancellation

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Shutting down.");

                _cancellationTokenSource.Cancel();  // Signal cancellation to observables
                udpReceiver?.Stop();                // Clean up Rx
                udpReceiver?.Dispose();             // Final cleanup
                ReleaseLock();
            };
        }
    }
}
