using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Defines the contract for broadcasting messages to multiple recipients.
    /// </summary>
    public interface IBroadcastMessage
    {
        /// <summary>
        /// Broadcasts a message to all connected clients or subscribers.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        void BroadcastMessage(Message message);
    }
}
