using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging
{
    /// <summary>
    /// Handles incoming messages by processing and optionally broadcasting them.
    /// </summary>
    public class MessageHandler
    {
        private readonly IBroadcastMessage _messageBroadcaster;

        /// <summary>
        /// Initializes a new instance with the given broadcaster.
        /// </summary>
        /// <param name="messageBroadcaster">The broadcaster to forward processed messages to.</param>
        public MessageHandler(IBroadcastMessage messageBroadcaster)
        {
            _messageBroadcaster = messageBroadcaster;
        }

        /// <summary>
        /// Processes and broadcasts a message using the underlying message type logic.
        /// </summary>
        /// <param name="message">The message to process.</param>
        public void HandleMessage(Message message)
        {
            // Delegate the processing to the IMessageType implementation
            message.Process();

            // Optionally, broadcast the message
            _messageBroadcaster.BroadcastMessage(message);
        }
    }
}
