using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging.MessageTypes
{
    /// <summary>
    /// Represents a message type used to indicate a disconnection event.
    /// </summary>
    public class DisconnectMessage : IMessageType
    {
        /// <summary>
        /// Handles the disconnect message by writing to the console.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="content">The content of the message.</param>
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"Message {sender} has disconnected");
        }
    }
}
