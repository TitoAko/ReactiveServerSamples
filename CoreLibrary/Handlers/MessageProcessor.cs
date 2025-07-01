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
        private readonly ICommunicator _communicator; // This will be TcpCommunicator or UdpCommunicator

        public MessageProcessor(LoggingService loggingService, IBroadcastMessage? broadcastMessage, ICommunicator communicator)
        {
            _loggingService = loggingService;
            _broadcastMessage = broadcastMessage;
            _communicator = communicator;
        }
        public MessageProcessor(LoggingService loggingService, IClient? client, ICommunicator communicator)
        {
            _loggingService = loggingService;
            _client = client;
            _communicator = communicator;
        }

        public void ProcessMessage(Message message)
        {
            // Example: Log the message content
            _loggingService.Log($"Processing message from {message.Sender}: {message.Content}");

            if (_client != null)
            {
                // If this is a client, we can send the message
                _client.ReceiveMessage(_communicator);
            }
            else if (_broadcastMessage != null)
            {
                // If this is a server, we can broadcast the message
                _broadcastMessage.BroadcastMessage(message);
            }
        }
    }
}
