using CoreLibrary.Interfaces;
using CoreLibrary.Messaging;

namespace ServerApp
{
    public class ChatServer : IBroadcastMessage, IMessageProcessor
    {
        private readonly ICommunicator _communicator;
        private readonly UserManager _userManager;

        public ChatServer(ICommunicator communicator, UserManager userManager)
        {
            _communicator = communicator;
            _userManager = userManager;
        }

        public void Start(IMessageProcessor messageProcessor)
        {
            // Start the server listening loop (accept clients, handle messages)
            _communicator.StartListening(messageProcessor);
        }

        // Broadcasting the message to all clients
        public void BroadcastMessage(Message message)
        {
            // Send a message to all connected clients
            foreach (var client in _userManager.GetAllClients())
            {
                client.SendMessage(message);
            }
        }

        // Process the message by broadcasting it to all clients
        public void ProcessMessage(Message message)
        {
            // Handle the logic for processing the message
            // For example, we can broadcast the message to all connected clients
            Console.WriteLine($"Processing message from {message.Sender}: {message.Content}");

            BroadcastMessage(message);  // Broadcast the message to all clients
        }
    }
}
