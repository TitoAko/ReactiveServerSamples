using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Utilities;

namespace CoreLibrary.Handlers
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly LoggingService _loggingService; // For logging purposes

        private readonly IBroadcastMessage? _broadcastMessage; // This will be ChatServer
        private readonly IClient? _client;  // This will be ChatClient

        public MessageProcessor(LoggingService loggingService, IBroadcastMessage? broadcastMessage)
        {
            _loggingService = loggingService;
            _broadcastMessage = broadcastMessage;
        }
        public MessageProcessor(LoggingService loggingService, IClient? client)
        {
            _loggingService = loggingService;
            _client = client;
        }

        public void ProcessMessage(Message message)
        {
            // Example: Log the message content
            _loggingService.Log($"Processing message from {message.Sender}: {message.Content}");

            if (_client != null)
            {
                // If this is a client, we can send the message
                _client.ReceiveMessage();
            }
            else if (_broadcastMessage != null)
            {
                // If this is a server, we can broadcast the message
                _broadcastMessage.BroadcastMessage(message);
            }
        }
    }
}
