using CoreLibrary.Interfaces;

namespace CoreLibrary.Utilities
{
    /// <summary>
    /// A simple console-based logger that implements <see cref="ILogger"/>.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Logs a message to the standard output with a prefix.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            // Log the message to the console
            Console.WriteLine($"[Log]: {message}");
        }
    }
}
