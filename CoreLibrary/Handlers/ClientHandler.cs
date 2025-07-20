using CoreLibrary.Messaging;

namespace CoreLibrary.Handlers
{
    /// <summary>
    /// Handles communication with a single client, receiving messages and passing them to the message handler.
    /// </summary>
    public class ClientHandler
    {
        // The client will handle these events
        /// <summary>
        /// Event triggered when a new message is received from the client.
        /// </summary>
        public event Action<Message>? OnMessageReceived;

        /// <summary>
        /// Event triggered when the client connects.
        /// </summary>
        public event Action? OnConnect;

        /// <summary>
        /// Event triggered when the client disconnects.
        /// </summary>
        public event Action? OnDisconnect;

        /// <summary>
        /// Handles a received message and notifies any subscribers.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void ReceiveMessage(Message message)
        {
            // Raise the event when a message is received
            OnMessageReceived?.Invoke(message);
        }

        /// <summary>
        /// Triggers the connect event.
        /// </summary>
        public void Connect()
        {
            // Raise the connection event
            OnConnect?.Invoke();
        }

        /// <summary>
        /// Triggers the disconnect event.
        /// </summary>
        public void Disconnect()
        {
            // Raise the disconnect event
            OnDisconnect?.Invoke();
        }
    }
}
