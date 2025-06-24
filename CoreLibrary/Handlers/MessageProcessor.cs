using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;
using CoreLibrary.Messaging.MessageTypes;
using CoreLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Handlers
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IBroadcastMessage _broadcastMessage; // Used to broadcast messages
        private readonly LoggingService _loggingService; // For logging purposes

        public MessageProcessor(IBroadcastMessage broadcastMessage, LoggingService loggingService)
        {
            _broadcastMessage = broadcastMessage;
            _loggingService = loggingService;
        }

        public void ProcessMessage(Message message)
        {
            // Example: Log the message content
            _loggingService.Log($"Processing message from {message.Sender}: {message.Content}");

            // After processing, broadcast the message
            _broadcastMessage.BroadcastMessage(message);
        }
    }
}
