using CoreLibrary.Utilities;

namespace ServerApp
{
    public class ServerAppInitializer
    {
        private readonly Configuration _config;
        private readonly AppLock _appLock;

        public ServerAppInitializer(Configuration config, AppLock appLock)
        {
            _config = config;  // Use Configuration to hold all the parameters
            _appLock = appLock;
        }

        public bool InitializeServer()
        {
            // Using ClientLock to check if the server is already running
            if (_appLock.IsInstanceRunning(_config))
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
