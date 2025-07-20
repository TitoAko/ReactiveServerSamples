using CoreLibrary.Interfaces;

namespace CoreLibrary.Utilities
{
    /// <summary>
    /// A wrapper for logger implementations. Currently forwards logs to the console.
    /// </summary>
    public class LoggingService
    {
        private readonly ILogger _logger;

        public LoggingService()
        {
            // Initialize the logger (we can choose a different logger implementation here, e.g., Console, File, etc.)
            _logger = new Logger();  // Example: simple console logger
        }

        /// <summary>
        /// Logs a message using the underlying logger implementation.
        /// </summary>
        public void Log(string message)
        {
            _logger.Log(message);
        }
    }
}
