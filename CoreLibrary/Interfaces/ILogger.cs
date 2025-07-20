namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Represents a logging mechanism for capturing and outputting messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified message using the underlying logging implementation.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);
    }
}
