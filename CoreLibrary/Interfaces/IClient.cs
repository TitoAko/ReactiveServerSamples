using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Defines methods for a client capable of sending, receiving, and displaying messages.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void SendMessage(Message message);

        /// <summary>
        /// Receives a message from the server.
        /// </summary>
        /// <returns>The message received.</returns>
        Message ReceiveMessage();

        /// <summary>
        /// Displays a received message to the user interface or console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void DisplayReceivedMessage(Message message);

        /// <summary>
        /// Establishes a connection to the server or communication endpoint.
        /// </summary>
        void Connect();

        /// <summary>
        /// Terminates the connection to the server or communication endpoint.
        /// </summary>
        void Disconnect();
    }
}
