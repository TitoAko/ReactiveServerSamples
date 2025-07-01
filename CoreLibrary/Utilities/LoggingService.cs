using CoreLibrary.Interfaces;

namespace CoreLibrary.Utilities
{
    public class LoggingService
    {
        private readonly ILogger _logger;

        public LoggingService()
        {
            // Initialize the logger (you can choose a different logger implementation here, e.g., Console, File, etc.)
            _logger = new Logger();  // Example: simple console logger
        }

        // Log a message (forwarding the call to the underlying logger)
        public void Log(string message)
        {
            _logger.Log(message);
        }
    }
}
