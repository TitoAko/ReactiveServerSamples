using CoreLibrary.Messaging;

namespace CoreLibrary.Handlers
{
    /// <summary>
    /// Handles communication with a single client, receiving messages and passing them to the message handler.
    /// </summary>
    public class ClientHandler
    {
        // Define an event to send a message (the client will handle this event)
        public event Action<Message>? OnMessageReceived;

        // Define other events if needed (e.g., OnConnect, OnDisconnect, etc.)
        public event Action? OnConnect; // Declared as nullable to resolve CS8618
        public event Action? OnDisconnect; // Declared as nullable to resolve CS8618

        public void ReceiveMessage(Message message)
        {
            // Raise the event when a message is received
            OnMessageReceived?.Invoke(message);
        }

        public void Connect()
        {
            // Raise the connection event
            OnConnect?.Invoke();
        }

        public void Disconnect()
        {
            // Raise the disconnect event
            OnDisconnect?.Invoke();
        }
    }
}
