using CoreLibrary.Utilities;

namespace ServerApp
{
    public class ServerAppInitializer
    {
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly AppLock _appLock;

        public ServerAppInitializer(string serverIp, int serverPort, AppLock appLock)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            _appLock = appLock;
        }

        public bool InitializeServer()
        {
            // Using ClientLock to check if the server is already running
            if (_appLock.IsInstanceRunning("server", _serverIp, _serverPort, "server"))
            {
                Console.WriteLine("The server is already running on this IP/Port.");
                return false;
            }

            // Initialize the server logic here...
            // Start the server
            Console.WriteLine("Server is starting...");

            return true;
        }

        public void ReleaseLock()
        {
            _appLock.ReleaseLock();  // Release the lock after the server finishes
        }
    }
}
