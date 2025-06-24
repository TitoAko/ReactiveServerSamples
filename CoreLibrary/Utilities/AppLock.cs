namespace CoreLibrary.Utilities
{
    public class AppLock
    {
        private Mutex _mutex;

        // Check if the client/server is already running for the given user/IP/port combination
        public bool IsInstanceRunning(string username, string ipAddress, int port, string appType = "client")
        {
            // Generate a unique mutex name based on user/IP/port and app type (client or server)
            string mutexName = $"AppLock_{username}_{ipAddress}_{port}_{appType}";

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
