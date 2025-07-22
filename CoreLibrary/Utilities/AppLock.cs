using System;
using System.Threading;

namespace CoreLibrary.Utilities
{
    /// <summary>
    /// Ensures single-instance enforcement based on a combination of user and app configuration.
    /// </summary>
    public class AppLock : IDisposable
    {
        private Mutex? _mutex;
        private bool _lockAcquired;

        /// <summary>
        /// Attempts to acquire a mutex based on the configuration.
        /// Returns true if another instance is already running.
        /// </summary>
        /// <param name="config">App configuration containing unique instance identifiers.</param>
        /// <returns>True if another instance is already running; otherwise, false.</returns>
        public bool IsInstanceRunning(Configuration config)
        {
            string mutexName = object.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? $"AppLock_{config.AppType}_{config.Port}_{config.AppName}"
                : $"AppLock_{config.Username}_{config.IpAddress}_{config.Port}_{config.AppType}"; _mutex = new Mutex(true, mutexName, out bool createdNew);
            _lockAcquired = createdNew;

            return !createdNew; // If already exists → another instance is running
        }

        /// <summary>
        /// Releases the acquired mutex if it belongs to this instance.
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

        /// <summary>
        /// Automatically releases the mutex during cleanup.
        /// </summary>
        public void Dispose()
        {
            ReleaseLock(); // Auto-clean if forgotten
        }
    }
}
