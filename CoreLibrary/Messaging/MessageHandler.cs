using CoreLibrary.Interfaces;

namespace CoreLibrary.Messaging
{
    public class MessageHandler
    {
        private readonly IBroadcastMessage _messageBroadcaster;

        public MessageHandler(IBroadcastMessage messageBroadcaster)
        {
            _messageBroadcaster = messageBroadcaster;
        }

        public void HandleMessage(Message message)
        {
            // Delegate the processing to the IMessageType implementation
            message.Process();

            // Optionally, broadcast the message
            _messageBroadcaster.BroadcastMessage(message);
        }
    }
}
