namespace CoreLibrary.Utilities
{
    public class AppLock
    {
        private Mutex? _mutex;

        // Check if the client/server is already running for the given user/IP/port combination
        public bool IsInstanceRunning(Configuration config)
        {
            // Generate a unique mutex name based on user/IP/port and app type (client or server)
            string mutexName = $"AppLock_{config.Username}_{config.IpAddress}_{config.Port}_{config.AppType}";

            // Attempt to create or open a mutex with the unique name
            _mutex = new Mutex(true, mutexName, out bool createdNew);

            return !createdNew;  // If the mutex already exists, another instance is running
        }

        // Release the mutex lock
        public void ReleaseLock()
        {
            _mutex?.ReleaseMutex();
        }
    }
}
