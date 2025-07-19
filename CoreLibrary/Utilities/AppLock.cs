using System;
using System.Threading;

namespace CoreLibrary.Utilities
{
    public class AppLock : IDisposable
    {
        private Mutex? _mutex;
        private bool _lockAcquired;

        /// <summary>
        /// Attempts to acquire a mutex based on the configuration.
        /// Returns true if this is the first instance running.
        /// </summary>
        public bool IsInstanceRunning(Configuration config)
        {
            string mutexName = $"AppLock_{config.Username}_{config.IpAddress}_{config.Port}_{config.AppType}";

            _mutex = new Mutex(true, mutexName, out bool createdNew);
            _lockAcquired = createdNew;

            return !createdNew; // If already exists → another instance is running
        }

        /// <summary>
        /// Releases the lock only if acquired by this instance.
        /// </summary>
        public void ReleaseLock()
        {
            if (_lockAcquired && _mutex != null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (ApplicationException)
                {
                    // Possibly already released — can be ignored or logged
                }
                finally
                {
                    _mutex.Dispose();
                    _mutex = null;
                    _lockAcquired = false;
                }
            }
        }

        public void Dispose()
        {
            ReleaseLock(); // Auto-clean if forgotten
        }
    }
}
