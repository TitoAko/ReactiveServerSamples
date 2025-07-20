namespace ClientApp
{
    /// <summary>
    /// Displays incoming messages from the server to the client,
    /// ensures messages are displayed in a user friendly format,
    /// and optionally, handle things like formatting or timestamping messages, depending on how the output is structured.
    /// </summary>
    public class OutputHandler : IDisposable
    {
        private bool _disposed = false;  // To detect redundant calls
        /// <summary>
        /// Disposes the <see cref="OutputHandler"/> and releases any held resources.
        /// Currently, no resources are held, so this is a placeholder for future extensions.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Console.WriteLine("OutputHandler disposed.");
                DisposeResources();  // Call to clean up resources if necessary
            }
            // If already disposed, do nothing
        }
        /// <summary>
        /// Dispose method to clean up resources if necessary
        /// </summary>
        public void DisposeResources()
        {
            Console.WriteLine("OutputHandler resources disposed.");
        }
        /// <summary>
        /// Display message (messageContent) received from the sender (who sent the message)
        /// </summary>
        /// <param name="sender">The origin of the message</param>
        /// <param name="messageContent">The actual message</param>
        public void DisplayMessage(string sender, string messageContent)
        {
            // Simple formatting for displaying the message to the console. Change this to display message in any other way
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {sender}: {messageContent}");
        }
    }
}
