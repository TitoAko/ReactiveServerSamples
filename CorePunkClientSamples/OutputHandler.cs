namespace ClientApp
{
    /// <summary>
    /// Displays incoming messages from the server to the client,
    /// ensures messages are displayed in a user friendly format,
    /// and optionally, handle things like formatting or timestamping messages, depending on how the output is structured.
    /// </summary>
    public class OutputHandler : IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }
        /// <summary>
        /// Dispose method to clean up resources if necessary
        /// </summary>
        public void DisposeResources()
        {
            this.DisposeResources();
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
